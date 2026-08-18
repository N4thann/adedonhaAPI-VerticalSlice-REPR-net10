using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogWordStatsUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogWordStatsUseCase> _loggerMock;
        private readonly GetCatalogWordStatsUseCase _sut;

        public GetCatalogWordStatsUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogWordStatsUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogWordStatsUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar o total de palavras e apenas as que estao em mais de uma categoria, ordenadas decrescente")]
        public async Task ExecuteAsync_WhenWordsExist_ShouldReturnTotalAndMultiCategoryWordsSortedDescending()
        {
            // Arrange
            Word carro = WordDataBuilder.Create().WithName("Carro").WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = "cat-1", Slug = "cat-1", Name = "Categoria 1" },
                new() { CategoryId = "cat-2", Slug = "cat-2", Name = "Categoria 2" },
                new() { CategoryId = "cat-3", Slug = "cat-3", Name = "Categoria 3" },
            });
            Word casa = WordDataBuilder.Create().WithName("Casa").WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = "cat-1", Slug = "cat-1", Name = "Categoria 1" },
                new() { CategoryId = "cat-2", Slug = "cat-2", Name = "Categoria 2" },
                new() { CategoryId = "cat-3", Slug = "cat-3", Name = "Categoria 3" },
                new() { CategoryId = "cat-4", Slug = "cat-4", Name = "Categoria 4" },
                new() { CategoryId = "cat-5", Slug = "cat-5", Name = "Categoria 5" },
            });
            Word unica = WordDataBuilder.Create().WithName("Unica").WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = "cat-1", Slug = "cat-1", Name = "Categoria 1" },
            });

            var words = new List<Word> { carro, casa, unica };

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(3);
            result.Value.WordsInMultipleCategories.Count.ShouldBe(2);
            result.Value.WordsInMultipleCategories[0].Slug.ShouldBe(casa.Slug);
            result.Value.WordsInMultipleCategories[0].CategoryCount.ShouldBe(5);
            result.Value.WordsInMultipleCategories[1].Slug.ShouldBe(carro.Slug);
            result.Value.WordsInMultipleCategories[1].CategoryCount.ShouldBe(3);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar lista vazia quando nenhuma palavra estiver em mais de uma categoria")]
        public async Task ExecuteAsync_WhenNoWordHasMultipleCategories_ShouldReturnEmptyList()
        {
            // Arrange
            var unica = WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = "cat-1", Slug = "cat-1", Name = "Categoria 1" },
            });

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { unica });

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(1);
            result.Value.WordsInMultipleCategories.ShouldBeEmpty();
        }
    }
}
