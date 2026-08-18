using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWordCounts;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogCategoryWordCountsUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogCategoryWordCountsUseCase> _loggerMock;
        private readonly GetCatalogCategoryWordCountsUseCase _sut;

        public GetCatalogCategoryWordCountsUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogCategoryWordCountsUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogCategoryWordCountsUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a contagem de palavras por categoria, ordenada decrescente, sem categorias com zero palavras")]
        public async Task ExecuteAsync_WhenCategoriesAndWordsExist_ShouldReturnCountsSortedDescendingExcludingZero()
        {
            // Arrange
            Category categoryA = CategoryDataBuilder.Create().WithName("Animais");
            Category categoryB = CategoryDataBuilder.Create().WithName("Objetos");
            Category categoryC = CategoryDataBuilder.Create().WithName("Vazia");

            var infoA = new Word.CategoryInfo { CategoryId = categoryA.Id, Slug = categoryA.Slug, Name = categoryA.Name };
            var infoB = new Word.CategoryInfo { CategoryId = categoryB.Id, Slug = categoryB.Slug, Name = categoryB.Name };

            var words = new List<Word>
            {
                WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo> { infoA }),
                WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo> { infoA }),
                WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo> { infoA }),
                WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo> { infoB }),
            };

            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { categoryA, categoryB, categoryC });
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoryWordCountsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Items.Count.ShouldBe(2);
            result.Value.Items[0].Slug.ShouldBe(categoryA.Slug);
            result.Value.Items[0].WordCount.ShouldBe(3);
            result.Value.Items[1].Slug.ShouldBe(categoryB.Slug);
            result.Value.Items[1].WordCount.ShouldBe(1);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar lista vazia quando nao houver categorias ativas")]
        public async Task ExecuteAsync_WhenNoCategoriesExist_ShouldReturnEmptyList()
        {
            // Arrange
            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category>());
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoryWordCountsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Items.ShouldBeEmpty();
        }
    }
}
