using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.GetWords;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class GetWordsUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IValidator<GetWordsInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetWordsUseCase> _loggerMock;
        private readonly GetWordsUseCase _sut;

        public GetWordsUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _validatorMock = Substitute.For<IValidator<GetWordsInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetWordsUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetWordsUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a pagina de palavras mapeada para WordSummary")]
        public async Task ExecuteAsync_WhenInputIsValid_ShouldReturnPagedWords()
        {
            // Arrange
            var input = new GetWordsInput(1, 10, null);
            var words = WordDataBuilder.AsList(2);
            var paged = new PagedResult<Word>(words, 2, 1, 10);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _wordRepoMock.GetPagedAsync(
                Arg.Any<Expression<Func<Word, bool>>>(),
                Arg.Any<Expression<Func<Word, object>>>(),
                Arg.Any<bool>(), 1, 10, Arg.Any<CancellationToken>()
            ).Returns(paged);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Items.Count.ShouldBe(2);
            result.Value.TotalCount.ShouldBe(2);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation quando PageSize exceder o limite")]
        public async Task ExecuteAsync_WhenPageSizeExceedsLimit_ShouldReturnValidationError()
        {
            // Arrange
            var input = new GetWordsInput(1, 1000, null);
            var failures = new List<ValidationFailure> { new("PageSize", "O tamanho da página deve estar entre 1 e 100.") };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(failures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Validation);
        }
    }
}
