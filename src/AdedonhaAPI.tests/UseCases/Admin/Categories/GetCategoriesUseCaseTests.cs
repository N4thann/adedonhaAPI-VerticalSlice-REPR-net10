using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Categories.GetCategories;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System.Linq.Expressions;

namespace AdedonhaAPI.tests.UseCases.Admin.Categories
{
    public class GetCategoriesUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IValidator<GetCategoriesInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCategoriesUseCase> _loggerMock;
        private readonly GetCategoriesUseCase _sut;

        public GetCategoriesUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _validatorMock = Substitute.For<IValidator<GetCategoriesInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCategoriesUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _sut = new GetCategoriesUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar a pagina de categorias mapeada para CategorySummary")]
        public async Task ExecuteAsync_WhenInputIsValid_ShouldReturnPagedCategories()
        {
            // Arrange
            var input = new GetCategoriesInput(1, 10, null);
            var categories = CategoryDataBuilder.AsList(3);
            var paged = new PagedResult<Category>(categories, 3, 1, 10);

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.GetPagedAsync(
                Arg.Any<Expression<Func<Category, bool>>>(),
                Arg.Any<Expression<Func<Category, object>>>(),
                Arg.Any<bool>(), 1, 10, Arg.Any<CancellationToken>()
            ).Returns(paged);

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Items.Count.ShouldBe(3);
            result.Value.TotalCount.ShouldBe(3);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation quando a pagina for invalida")]
        public async Task ExecuteAsync_WhenPageIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var input = new GetCategoriesInput(0, 10, null);
            var failures = new List<ValidationFailure> { new("Page", "A página deve ser maior ou igual a 1.") };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(failures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Validation);

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().GetPagedAsync(default!, default!, default, default, default, default);
        }
    }
}
