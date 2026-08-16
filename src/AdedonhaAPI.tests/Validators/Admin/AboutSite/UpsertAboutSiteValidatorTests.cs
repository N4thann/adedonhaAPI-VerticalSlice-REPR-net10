using AdedonhaAPI.Application.Common.Options;
using AdedonhaAPI.Application.Common.Storage;
using AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite;
using Microsoft.Extensions.Options;
using Shouldly;

namespace AdedonhaAPI.tests.Validators.Admin.AboutSite
{
    public class UpsertAboutSiteValidatorTests
    {
        private readonly UpsertAboutSiteValidator _sut = new(Options.Create(new FileStorageOptions()));

        [Fact(DisplayName = "SUCESSO - Deve validar quando cargo e texto geral forem informados e não houver imagem")]
        public async Task ValidateAsync_WhenRequiredFieldsAreProvidedAndNoImage_ShouldNotReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("Engenheiro de Software", new List<string> { "Ciência da Computação" }, "Texto geral", new List<string>(), new List<string>(), null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeTrue();
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando o cargo for vazio")]
        public async Task ValidateAsync_WhenCargoIsEmpty_ShouldReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("", new List<string>(), "Texto geral", new List<string>(), new List<string>(), null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == nameof(UpsertAboutSiteInput.Cargo));
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando o texto geral for vazio")]
        public async Task ValidateAsync_WhenTextoGeralIsEmpty_ShouldReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "", new List<string>(), new List<string>(), null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == nameof(UpsertAboutSiteInput.TextoGeral));
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando um item da lista de formações for vazio")]
        public async Task ValidateAsync_WhenAFormacaoItemIsEmpty_ShouldReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("Cargo", new List<string> { "Curso A", "" }, "Texto geral", new List<string>(), new List<string>(), null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName.StartsWith(nameof(UpsertAboutSiteInput.Formacoes)));
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando um item da lista de tecnologias for vazio")]
        public async Task ValidateAsync_WhenATecnologiaItemIsEmpty_ShouldReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "Texto geral", new List<string> { ".NET", "" }, new List<string>(), null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName.StartsWith(nameof(UpsertAboutSiteInput.Tecnologias)));
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando um item da lista de arquiteturas for vazio")]
        public async Task ValidateAsync_WhenAArquiteturaItemIsEmpty_ShouldReturnError()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "Texto geral", new List<string>(), new List<string> { "Clean Architecture", "" }, null);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName.StartsWith(nameof(UpsertAboutSiteInput.Arquiteturas)));
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando o Content-Type da imagem não for permitido")]
        public async Task ValidateAsync_WhenImageContentTypeIsNotAllowed_ShouldReturnError()
        {
            // Arrange
            var image = new FileUploadDto(new MemoryStream(), "arquivo.gif", "image/gif", 1024);
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "Texto geral", new List<string>(), new List<string>(), image);

            // Act
            var result = await _sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Image.ContentType");
        }

        [Fact(DisplayName = "ERRO - Deve rejeitar quando a imagem exceder o tamanho máximo configurado")]
        public async Task ValidateAsync_WhenImageExceedsMaxSize_ShouldReturnError()
        {
            // Arrange
            var options = Options.Create(new FileStorageOptions { MaxFileSizeBytes = 1000 });
            var sut = new UpsertAboutSiteValidator(options);

            var image = new FileUploadDto(new MemoryStream(), "foto.jpg", "image/jpeg", 1001);
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "Texto geral", new List<string>(), new List<string>(), image);

            // Act
            var result = await sut.ValidateAsync(input);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Image.Length");
        }
    }
}
