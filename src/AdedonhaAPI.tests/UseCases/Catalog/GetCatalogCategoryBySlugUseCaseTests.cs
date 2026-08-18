using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryBySlug;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogCategoryBySlugUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogCategoryBySlugUseCase> _loggerMock;
        private readonly GetCatalogCategoryBySlugUseCase _sut;

        public GetCatalogCategoryBySlugUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogCategoryBySlugUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogCategoryBySlugUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a categoria com as letras disponiveis quando o slug existir e estiver ativa")]
        public async Task ExecuteAsync_WhenCategoryExistsAndIsActive_ShouldReturnCategoryWithAvailableLetters()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithName("Animais");
            var words = new List<Word>
            {
                WordDataBuilder.Create().WithName("Abelha"),
                WordDataBuilder.Create().WithName("Aranha"),
                WordDataBuilder.Create().WithName("Baleia"),
            };

            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { category });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoryBySlugInput(category.Slug), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Slug.ShouldBe(category.Slug);
            result.Value.AvailableLetters.ShouldBe(new[] { 'A', 'B' });
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar lista vazia de letras quando a categoria nao tiver nenhuma palavra")]
        public async Task ExecuteAsync_WhenCategoryHasNoWords_ShouldReturnEmptyAvailableLetters()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { category });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoryBySlugInput(category.Slug), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.AvailableLetters.ShouldBeEmpty();
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o slug nao existir")]
        public async Task ExecuteAsync_WhenSlugDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoryBySlugInput("slug-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Category.NotFound");

            await _wordRepoMock.DidNotReceiveWithAnyArgs().FindAsync(default!, default);
        }
    }
}
