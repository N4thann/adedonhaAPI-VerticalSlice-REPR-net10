using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Categories.DeleteCategory;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Categories
{
    public class DeleteCategoryUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRepository<Word> _wordRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<DeleteCategoryUseCase> _loggerMock;
        private readonly DeleteCategoryUseCase _sut;

        public DeleteCategoryUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _wordRepoMock = Substitute.For<IRepository<Word>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<DeleteCategoryUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _unitOfWorkMock.Words.Returns(_wordRepoMock);
            _sut = new DeleteCategoryUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve fazer soft delete quando nao houver palavra associada")]
        public async Task ExecuteAsync_WhenNoWordsAssociated_ShouldSoftDeleteCategory()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithIsActive(true);
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word>());

            // Act
            var result = await _sut.ExecuteAsync(new DeleteCategoryInput(category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            await _categoryRepoMock.Received(1).UpdateAsync(Arg.Is<Category>(c => c.IsActive == false), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar Conflict quando houver palavra ativa associada")]
        public async Task ExecuteAsync_WhenActiveWordsAssociated_ShouldReturnConflict()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithIsActive(true);
            Word associatedWord = WordDataBuilder.Create();
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
            _wordRepoMock.FindAsync(Arg.Any<Expression<Func<Word, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Word> { associatedWord });

            // Act
            var result = await _sut.ExecuteAsync(new DeleteCategoryInput(category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("Category.Conflict.HasAssociatedWords");

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria nao existir")]
        public async Task ExecuteAsync_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _categoryRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _sut.ExecuteAsync(new DeleteCategoryInput("id-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
        }
    }
}
