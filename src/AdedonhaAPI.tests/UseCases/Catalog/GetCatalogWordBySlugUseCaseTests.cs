using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogWordBySlug;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogWordBySlugUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogWordBySlugUseCase> _loggerMock;
        private readonly GetCatalogWordBySlugUseCase _sut;

        public GetCatalogWordBySlugUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogWordBySlugUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new GetCatalogWordBySlugUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a palavra com suas categorias quando o slug existir e estiver ativa")]
        public async Task ExecuteAsync_WhenWordExistsAndIsActive_ShouldReturnWordWithCategories()
        {
            // Arrange
            var categories = new List<Word.CategoryInfo>
            {
                new() { CategoryId = "cat-1", Slug = "animais", Name = "Animais" },
                new() { CategoryId = "cat-2", Slug = "natureza", Name = "Natureza" },
            };
            Word word = WordDataBuilder.Create().WithName("Baleia").WithCategories(categories);

            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { word });

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordBySlugInput(word.Slug), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Name.ShouldBe("Baleia");
            result.Value.Categories.Count.ShouldBe(2);
            result.Value.Categories.ShouldContain(c => c.Slug == "animais" && c.Name == "Animais");
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra nao existir ou estiver inativa")]
        public async Task ExecuteAsync_WhenWordDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogWordBySlugInput("slug-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Word.NotFound");
        }
    }
}
