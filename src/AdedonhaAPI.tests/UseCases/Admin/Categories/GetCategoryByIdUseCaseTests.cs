using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryById;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.Categories
{
    public class GetCategoryByIdUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCategoryByIdUseCase> _loggerMock;
        private readonly GetCategoryByIdUseCase _sut;

        public GetCategoryByIdUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCategoryByIdUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _sut = new GetCategoryByIdUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a categoria quando o id existir e estiver ativa")]
        public async Task ExecuteAsync_WhenCategoryExistsAndIsActive_ShouldReturnCategory()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithIsActive(true);
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _sut.ExecuteAsync(new GetCategoryByIdInput(category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Id.ShouldBe(category.Id);
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando o id nao existir")]
        public async Task ExecuteAsync_WhenCategoryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _categoryRepoMock.GetByIdAsync("id-inexistente", Arg.Any<CancellationToken>()).Returns((Category?)null);

            // Act
            var result = await _sut.ExecuteAsync(new GetCategoryByIdInput("id-inexistente"), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
            result.FirstError.Code.ShouldBe("Category.NotFound");
        }

        [Fact(DisplayName = "ERRO - Deve retornar NotFound quando a categoria estiver inativa (soft deletada)")]
        public async Task ExecuteAsync_WhenCategoryIsInactive_ShouldReturnNotFound()
        {
            // Arrange
            Category category = CategoryDataBuilder.Create().WithIsActive(false);
            _categoryRepoMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

            // Act
            var result = await _sut.ExecuteAsync(new GetCategoryByIdInput(category.Id), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
        }
    }
}
