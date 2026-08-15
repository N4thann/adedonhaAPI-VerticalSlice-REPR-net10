using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogCategoriesUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogCategoriesUseCase> _loggerMock;
        private readonly GetCatalogCategoriesUseCase _sut;

        public GetCatalogCategoriesUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogCategoriesUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogCategoriesUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar cada categoria ativa com ate 10 palavras de amostra")]
        public async Task ExecuteAsync_WhenCategoriesExist_ShouldReturnEachWithSampleWords()
        {
            // Arrange
            var categories = CategoryDataBuilder.AsList(2);
            var sampleWords = WordDataBuilder.AsList(10);

            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(categories);
            _wordRepoMock.GetRandomSampleAsync(Arg.Any<Expression<Func<Word, bool>>>(), 10, Arg.Any<CancellationToken>())
                .Returns(sampleWords);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoriesInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Categories.Count.ShouldBe(2);
            result.Value.Categories[0].SampleWords.Count.ShouldBe(10);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar menos de 10 palavras quando a categoria tiver poucas palavras ativas")]
        public async Task ExecuteAsync_WhenCategoryHasFewActiveWords_ShouldReturnOnlyThose()
        {
            // Arrange
            var categories = CategoryDataBuilder.AsList(1);
            var sampleWords = WordDataBuilder.AsList(3);

            _categoryRepoMock.FindAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(categories);
            _wordRepoMock.GetRandomSampleAsync(Arg.Any<Expression<Func<Word, bool>>>(), 10, Arg.Any<CancellationToken>())
                .Returns(sampleWords);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogCategoriesInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Categories[0].SampleWords.Count.ShouldBe(3);
        }
    }
}
