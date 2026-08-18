using AdedonhaAPI.Application.Features.Admin.Words.GetWords;
using Shouldly;

namespace AdedonhaAPI.tests.Validators.Admin.Words
{
    public class GetWordsValidatorTests
    {
        private readonly GetWordsValidator _sut = new();

        [Fact(DisplayName = "SUCESSO - Deve validar quando CategoryId nao for informado")]
        public async Task ValidateAsync_WhenCategoryIdIsNull_ShouldBeValid()
        {
            // Arrange
            var input = new GetWordsInput(1, 10, null, null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact(DisplayName = "ERRO - Deve invalidar quando CategoryId for string vazia")]
        public async Task ValidateAsync_WhenCategoryIdIsEmpty_ShouldBeInvalid()
        {
            // Arrange
            var input = new GetWordsInput(1, 10, null, "");

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveSingleItem();
            result.Errors[0].PropertyName.ShouldBe("CategoryId");
        }
    }
}
