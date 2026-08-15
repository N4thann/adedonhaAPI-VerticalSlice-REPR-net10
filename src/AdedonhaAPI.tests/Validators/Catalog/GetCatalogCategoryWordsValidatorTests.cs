using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords;
using Shouldly;

namespace AdedonhaAPI.tests.Validators.Catalog
{
    public class GetCatalogCategoryWordsValidatorTests
    {
        private readonly GetCatalogCategoryWordsValidator _sut = new();

        [Fact(DisplayName = "ERRO - Deve invalidar quando Page for menor que 1")]
        public async Task Validate_WhenPageIsLessThanOne_ShouldBeInvalid()
        {
            // Arrange
            var input = new GetCatalogCategoryWordsInput("categoria", 0, 20, null, null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
        }

        [Fact(DisplayName = "ERRO - Deve invalidar quando Letter nao for uma letra")]
        public async Task Validate_WhenLetterIsNotALetter_ShouldBeInvalid()
        {
            // Arrange
            var input = new GetCatalogCategoryWordsInput("categoria", 1, 20, '5', null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
        }

        [Fact(DisplayName = "SUCESSO - Deve validar quando todos os campos forem validos")]
        public async Task Validate_WhenAllFieldsAreValid_ShouldBeValid()
        {
            // Arrange
            var input = new GetCatalogCategoryWordsInput("categoria", 1, 20, 'A', "busca");

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeTrue();
        }
    }
}
