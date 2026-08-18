using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.GetWordStats;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class GetWordStatsUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetWordStatsUseCase> _loggerMock;
        private readonly GetWordStatsUseCase _sut;

        public GetWordStatsUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetWordStatsUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetWordStatsUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar o total de palavras e as que estao em mais de uma categoria, ordenadas decrescente")]
        public async Task ExecuteAsync_WhenWordsExist_ShouldReturnTotalAndMultiCategoryWordsSortedDescending()
        {
            // Arrange
            var categoryA = new Word.CategoryInfo { CategoryId = "cat-a", Slug = "animais", Name = "Animais" };
            var categoryB = new Word.CategoryInfo { CategoryId = "cat-b", Slug = "natureza", Name = "Natureza" };
            var categoryC = new Word.CategoryInfo { CategoryId = "cat-c", Slug = "objetos", Name = "Objetos" };

            Word wordWithThree = WordDataBuilder.Create().WithName("Baleia")
                .WithCategories(new List<Word.CategoryInfo> { categoryA, categoryB, categoryC });
            Word wordWithTwo = WordDataBuilder.Create().WithName("Leão")
                .WithCategories(new List<Word.CategoryInfo> { categoryA, categoryB });
            Word wordWithOne = WordDataBuilder.Create().WithName("Cadeira")
                .WithCategories(new List<Word.CategoryInfo> { categoryC });

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { wordWithThree, wordWithTwo, wordWithOne });

            // Act
            var result = await _sut.ExecuteAsync(new GetWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(3);
            result.Value.WordsInMultipleCategories.Count.ShouldBe(2);
            result.Value.WordsInMultipleCategories[0].Slug.ShouldBe(wordWithThree.Slug);
            result.Value.WordsInMultipleCategories[0].CategoryCount.ShouldBe(3);
            result.Value.WordsInMultipleCategories[1].Slug.ShouldBe(wordWithTwo.Slug);
            result.Value.WordsInMultipleCategories[1].CategoryCount.ShouldBe(2);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar lista vazia quando nenhuma palavra estiver em mais de uma categoria")]
        public async Task ExecuteAsync_WhenNoWordInMultipleCategories_ShouldReturnEmptyList()
        {
            // Arrange
            var category = new Word.CategoryInfo { CategoryId = "cat-a", Slug = "animais", Name = "Animais" };
            Word word = WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo> { category });

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { word });

            // Act
            var result = await _sut.ExecuteAsync(new GetWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(1);
            result.Value.WordsInMultipleCategories.ShouldBeEmpty();
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar zero quando nao houver palavras ativas")]
        public async Task ExecuteAsync_WhenNoWordsExist_ShouldReturnZero()
        {
            // Arrange
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new GetWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(0);
            result.Value.WordsInMultipleCategories.ShouldBeEmpty();
        }
    }
}
