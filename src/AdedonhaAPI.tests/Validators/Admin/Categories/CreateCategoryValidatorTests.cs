using AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory;
using Shouldly;

namespace AdedonhaAPI.tests.Validators.Admin.Categories
{
    public class CreateCategoryValidatorTests
    {
        private readonly CreateCategoryValidator _sut = new();

        [Fact(DisplayName = "ERRO - Deve invalidar quando Name for vazio")]
        public async Task Validate_WhenNameIsEmpty_ShouldBeInvalid()
        {
            // Arrange
            var input = new CreateCategoryInput("", null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Name");
        }

        [Fact(DisplayName = "SUCESSO - Deve validar quando Name for preenchido")]
        public async Task Validate_WhenNameIsFilled_ShouldBeValid()
        {
            // Arrange
            var input = new CreateCategoryInput("Animais", null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeTrue();
        }
    }
}
