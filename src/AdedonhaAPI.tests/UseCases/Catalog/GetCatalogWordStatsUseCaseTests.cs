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

        [Fact(DisplayName = "SUCESSO - Deve retornar o total de palavras ativas")]
        public async Task ExecuteAsync_WhenWordsExist_ShouldReturnTotalWords()
        {
            // Arrange
            var words = WordDataBuilder.AsList(3);

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(3);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar zero quando nao houver palavras ativas")]
        public async Task ExecuteAsync_WhenNoWordsExist_ShouldReturnZero()
        {
            // Arrange
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(0);
        }
    }
}
