using AdedonhaAPI.Domain.Common;
using Shouldly;

namespace AdedonhaAPI.tests.Common
{
    public class SlugGeneratorTests
    {
        [Theory(DisplayName = "SUCESSO - Deve gerar slug em minusculas, sem acento, com hifen no lugar de espaco")]
        [InlineData("Água", "agua")]
        [InlineData("Ação", "acao")]
        [InlineData("Carro Esportivo", "carro-esportivo")]
        [InlineData("  Espaco Extra  ", "espaco-extra")]
        public void Generate_WhenCalledWithText_ShouldReturnNormalizedSlug(string input, string expected)
        {
            // Act
            var result = SlugGenerator.Generate(input);

            // Assert
            result.ShouldBe(expected);
        }

        [Theory(DisplayName = "SUCESSO - Deve retornar a primeira letra em maiusculo e sem acento")]
        [InlineData("Água", 'A')]
        [InlineData("banana", 'B')]
        [InlineData("Élan", 'E')]
        public void GetInitialLetter_WhenCalledWithText_ShouldReturnUppercaseFirstLetterWithoutDiacritics(string input, char expected)
        {
            // Act
            var result = SlugGenerator.GetInitialLetter(input);

            // Assert
            result.ShouldBe(expected);
        }
    }
}
