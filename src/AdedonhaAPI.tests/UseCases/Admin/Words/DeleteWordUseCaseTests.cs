using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.DeleteWord;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class DeleteWordUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<DeleteWordUseCase> _loggerMock;
        private readonly DeleteWordUseCase _sut;

        public DeleteWordUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<DeleteWordUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new DeleteWordUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve fazer soft delete da palavra")]
        public async Task ExecuteAsync_WhenWordExists_ShouldSoftDeleteWord()
        {
            // Arrange
            Word word = WordDataBuilder.Create().WithIsActive(true);
            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);

            // Act
            var result = await _sut.ExecuteAsync(new DeleteWordInput(word.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            await _wordRepoMock.Received(1).UpdateAsync(Arg.Is<Word>(w => w.IsActive == false), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra nao existir")]
        public async Task ExecuteAsync_WhenWordDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _wordRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Word?)null);

            // Act
            var result = await _sut.ExecuteAsync(new DeleteWordInput("id-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }
    }
}
