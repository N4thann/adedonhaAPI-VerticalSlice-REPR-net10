using AdedonhaAPI.Application.Features.Admin.Words.CreateWord;
using Shouldly;

namespace AdedonhaAPI.tests.Validators.Admin.Words
{
    public class CreateWordValidatorTests
    {
        private readonly CreateWordValidator _sut = new();

        [Fact(DisplayName = "ERRO - Deve invalidar quando Name for vazio")]
        public async Task Validate_WhenNameIsEmpty_ShouldBeInvalid()
        {
            // Arrange
            var input = new CreateWordInput("", null, null);

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
            var input = new CreateWordInput("Adidas", null, null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeTrue();
        }
    }
}
