using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.Categories
{
    public class CreateCategoryUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<Category> _categoryRepoMock;
        private readonly IValidator<CreateCategoryInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<CreateCategoryUseCase> _loggerMock;
        private readonly CreateCategoryUseCase _sut;

        public CreateCategoryUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _categoryRepoMock = Substitute.For<IRepository<Category>>();
            _validatorMock = Substitute.For<IValidator<CreateCategoryInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<CreateCategoryUseCase>>();
            _unitOfWorkMock.Categories.Returns(_categoryRepoMock);
            _sut = new CreateCategoryUseCase(_unitOfWorkMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve criar a categoria quando o nome for unico e valido")]
        public async Task ExecuteAsync_WhenNameIsValidAndUnique_ShouldCreateCategory()
        {
            // Arrange
            var input = new CreateCategoryInput("Animais", "Categoria de animais");

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category>());

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Name.ShouldBe("Animais");
            result.Value.Slug.ShouldBe("animais");

            await _categoryRepoMock.Received(1).AddAsync(Arg.Is<Category>(c =>
                c.Name == "Animais" && c.Slug == "animais" && c.Description == "Categoria de animais"
            ), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation quando o nome for vazio")]
        public async Task ExecuteAsync_WhenNameIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var input = new CreateCategoryInput("", null);
            var failures = new List<ValidationFailure> { new("Name", "O nome da categoria é obrigatório.") };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(failures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Validation);
            result.FirstError.Code.ShouldBe("Name");

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }

        [Fact(DisplayName = "ERRO - Deve retornar Conflict quando ja existir categoria com o mesmo nome")]
        public async Task ExecuteAsync_WhenNameAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var input = new CreateCategoryInput("Animais", null);
            Category existing = CategoryDataBuilder.Create().WithName("Animais");

            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());
            _categoryRepoMock.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Category> { existing });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("Category.Conflict.NameAlreadyExists");

            await _categoryRepoMock.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        }
    }
}
