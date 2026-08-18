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

        [Fact(DisplayName = "SUCESSO - Deve retornar o total de palavras ativas")]
        public async Task ExecuteAsync_WhenWordsExist_ShouldReturnTotalWords()
        {
            // Arrange
            var words = WordDataBuilder.AsList(3);

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(words);

            // Act
            var result = await _sut.ExecuteAsync(new GetWordStatsInput(), CancellationToken.None);

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
            var result = await _sut.ExecuteAsync(new GetWordStatsInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.TotalWords.ShouldBe(0);
        }
    }
}
