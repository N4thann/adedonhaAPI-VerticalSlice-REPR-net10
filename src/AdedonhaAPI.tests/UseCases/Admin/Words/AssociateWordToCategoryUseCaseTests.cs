using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Words.AssociateWordToCategory;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.Words
{
    public class AssociateWordToCategoryUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<AssociateWordToCategoryUseCase> _loggerMock;
        private readonly AssociateWordToCategoryUseCase _sut;

        public AssociateWordToCategoryUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<AssociateWordToCategoryUseCase>>();
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _sut = new AssociateWordToCategoryUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve associar a palavra a categoria quando ainda nao estiver associada")]
        public async Task ExecuteAsync_WhenNotYetAssociated_ShouldAddCategoryToWord()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            Word word = WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo>());

            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _sut.ExecuteAsync(new AssociateWordToCategoryInput(word.Id, category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.CategoryIds.ShouldContain(category.Id);

            await _wordRepoMock.Received(1).UpdateAsync(Arg.Is<Word>(w => w.Categories.Any(c => c.CategoryId == category.Id)), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "SUCESSO - Deve ser idempotente quando a palavra ja estiver associada a categoria")]
        public async Task ExecuteAsync_WhenAlreadyAssociated_ShouldNotDuplicateAndNotError()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create();
            Word word = WordDataBuilder.Create().WithCategories(new List<Word.CategoryInfo>
            {
                new() { CategoryId = category.Id, Slug = category.Slug, Name = category.Name }
            });

            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _sut.ExecuteAsync(new AssociateWordToCategoryInput(word.Id, category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.CategoryIds.Count.ShouldBe(1);

            await _wordRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a palavra nao existir")]
        public async Task ExecuteAsync_WhenWordDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _wordRepoMock.GetByIdAsync("word-inexistente", Arg.Any<CancellationToken>()).Returns((Word?)null);

            // Act
            var result = await _sut.ExecuteAsync(new AssociateWordToCategoryInput("word-inexistente", "cat-1"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Word.NotFound");
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria nao existir")]
        public async Task ExecuteAsync_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            Word word = WordDataBuilder.Create();
            _wordRepoMock.GetByIdAsync(word.Id, Arg.Any<CancellationToken>()).Returns(word);
            _categoryRepoMock.GetByIdAsync("cat-inexistente", Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _sut.ExecuteAsync(new AssociateWordToCategoryInput(word.Id, "cat-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Category.NotFound");
        }
    }
}
