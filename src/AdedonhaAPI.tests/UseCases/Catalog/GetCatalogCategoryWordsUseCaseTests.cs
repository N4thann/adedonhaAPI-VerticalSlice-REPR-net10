using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogCategoryWordsUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IValidator<GetCatalogCategoryWordsInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogCategoryWordsUseCase> _loggerMock;
        private readonly GetCatalogCategoryWordsUseCase _sut;

        public GetCatalogCategoryWordsUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _validatorMock = Substitute.For<IValidator<GetCatalogCategoryWordsInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogCategoryWordsUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogCategoryWordsUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a pagina de palavras da categoria")]
        public async Task ExecuteAsync_WhenCategoryExists_ShouldReturnPagedWords()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            var input = new GetCatalogCategoryWordsInput(category.Slug, 1, 20, null, null, 42);
            var words = WordDataBuilder.AsList(5);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { category });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Items.Count.ShouldBe(5);
            result.Value.TotalCount.ShouldBe(5);
        }

        [Fact(DisplayName = "SUCESSO - Mesmo seed deve produzir a mesma ordem em chamadas diferentes")]
        public async Task ExecuteAsync_WhenSameSeed_ShouldReturnSameOrder()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            var words = WordDataBuilder.AsList(10);
            var inputA = new GetCatalogCategoryWordsInput(category.Slug, 1, 10, null, null, 777);
            var inputB = new GetCatalogCategoryWordsInput(category.Slug, 1, 10, null, null, 777);

            _validatorMock.ValidateAsync(Arg.Any<GetCatalogCategoryWordsInput>(), Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { category });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var resultA = await _sut.ExecuteAsync(inputA, CancellationToken.None);
            var resultB = await _sut.ExecuteAsync(inputB, CancellationToken.None);

            // Assert
            resultA.Value.Items.Select(i => i.Slug).ShouldBe(resultB.Value.Items.Select(i => i.Slug));
        }

        [Fact(DisplayName = "SUCESSO - Seeds diferentes devem produzir ordens diferentes")]
        public async Task ExecuteAsync_WhenDifferentSeeds_ShouldReturnDifferentOrder()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            var words = WordDataBuilder.AsList(10);
            var inputA = new GetCatalogCategoryWordsInput(category.Slug, 1, 10, null, null, 111);
            var inputB = new GetCatalogCategoryWordsInput(category.Slug, 1, 10, null, null, 222);

            _validatorMock.ValidateAsync(Arg.Any<GetCatalogCategoryWordsInput>(), Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { category });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var resultA = await _sut.ExecuteAsync(inputA, CancellationToken.None);
            var resultB = await _sut.ExecuteAsync(inputB, CancellationToken.None);

            // Assert
            resultA.Value.Items.Select(i => i.Slug).ShouldNotBe(resultB.Value.Items.Select(i => i.Slug));
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria nao existir")]
        public async Task ExecuteAsync_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var input = new GetCatalogCategoryWordsInput("slug-inexistente", 1, 20, null, null, 42);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category>());

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().FindAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation quando a pagina for invalida")]
        public async Task ExecuteAsync_WhenPageIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var input = new GetCatalogCategoryWordsInput("qualquer-slug", 0, 20, null, null, 42);
            var failures = new List<ValidationFailure> { new("Page", "A página deve ser maior ou igual a 1.") };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(failures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Validation);

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().FindAsync(default!, default);
        }
    }
}
