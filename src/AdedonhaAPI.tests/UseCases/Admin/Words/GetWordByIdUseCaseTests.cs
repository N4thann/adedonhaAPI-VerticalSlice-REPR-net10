using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.GetWordById;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class GetWordByIdUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetWordByIdUseCase> _loggerMock;
        private readonly GetWordByIdUseCase _sut;

        public GetWordByIdUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetWordByIdUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetWordByIdUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a palavra quando o id existir e estiver ativa")]
        public async Task ExecuteAsync_WhenWordExistsAndIsActive_ShouldReturnWord()
        {
            // Arrange
            Word word = WordDataBuilder.Create().WithIsActive(true);
            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);

            // Act
            var result = await _sut.ExecuteAsync(new GetWordByIdInput(word.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Id.ShouldBe(word.Id);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id nao existir")]
        public async Task ExecuteAsync_WhenWordDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _wordRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Word?)null);

            // Act
            var result = await _sut.ExecuteAsync(new GetWordByIdInput("id-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Word.NotFound");
        }
    }
}
