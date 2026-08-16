using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Storage;
using AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Admin.AboutSite
{
    public class UpsertAboutSiteUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<AboutSiteContent> _repoMock;
        private readonly IFileStorageService _fileStorageServiceMock;
        private readonly IValidator<UpsertAboutSiteInput> _validatorMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<UpsertAboutSiteUseCase> _loggerMock;
        private readonly UpsertAboutSiteUseCase _sut;

        public UpsertAboutSiteUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _repoMock = Substitute.For<IRepository<AboutSiteContent>>();
            _fileStorageServiceMock = Substitute.For<IFileStorageService>();
            _validatorMock = Substitute.For<IValidator<UpsertAboutSiteInput>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<UpsertAboutSiteUseCase>>();

            _unitOfWorkMock.AboutSite.Returns(_repoMock);

            _sut = new UpsertAboutSiteUseCase(_unitOfWorkMock, _fileStorageServiceMock, _validatorMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve criar um novo registro quando ainda não existir nenhum")]
        public async Task ExecuteAsync_WhenNoContentExistsYet_ShouldCreateNewContent()
        {
            // Arrange
            var input = new UpsertAboutSiteInput(
                "Cargo",
                new List<string> { "Formação A" },
                "Texto geral",
                new List<string> { ".NET", "MongoDB" },
                new List<string> { "Vertical Slice" },
                null);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

            _repoMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<AboutSiteContent>());

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Cargo.ShouldBe("Cargo");
            result.Value.Tecnologias.ShouldBe(new List<string> { ".NET", "MongoDB" });
            result.Value.Arquiteturas.ShouldBe(new List<string> { "Vertical Slice" });
            result.Value.ImageUrl.ShouldBeNull();

            await _repoMock.Received(1).AddAsync(Arg.Is<AboutSiteContent>(c =>
                c.Cargo == "Cargo" && c.TextoGeral == "Texto geral" && c.ImageUrl == null &&
                c.Tecnologias.SequenceEqual(new List<string> { ".NET", "MongoDB" }) &&
                c.Arquiteturas.SequenceEqual(new List<string> { "Vertical Slice" })
            ), Arg.Any<CancellationToken>());
            await _fileStorageServiceMock.DidNotReceiveWithAnyArgs().SaveAsync(default!, default);
        }

        [Fact(DisplayName = "SUCESSO - Deve manter a imagem já cadastrada quando nenhuma imagem nova for enviada")]
        public async Task ExecuteAsync_WhenNoNewImageIsSent_ShouldKeepExistingImageUrl()
        {
            // Arrange
            AboutSiteContent existing = AboutSiteContentDataBuilder.Create().WithImageUrl("/uploads/antiga.jpg");
            var input = new UpsertAboutSiteInput(
                "Cargo novo",
                new List<string>(),
                "Texto novo",
                new List<string> { "React" },
                new List<string> { "MVC" },
                null);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

            _repoMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<AboutSiteContent> { existing });

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.ImageUrl.ShouldBe("/uploads/antiga.jpg");
            result.Value.Tecnologias.ShouldBe(new List<string> { "React" });
            result.Value.Arquiteturas.ShouldBe(new List<string> { "MVC" });

            await _repoMock.Received(1).UpdateAsync(Arg.Is<AboutSiteContent>(c =>
                c.ImageUrl == "/uploads/antiga.jpg" &&
                c.Tecnologias.SequenceEqual(new List<string> { "React" }) &&
                c.Arquiteturas.SequenceEqual(new List<string> { "MVC" })
            ), Arg.Any<CancellationToken>());
            await _fileStorageServiceMock.DidNotReceiveWithAnyArgs().SaveAsync(default!, default);
            await _fileStorageServiceMock.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        }

        [Fact(DisplayName = "SUCESSO - Deve salvar a imagem nova e apagar a anterior quando uma imagem for enviada")]
        public async Task ExecuteAsync_WhenNewImageIsSent_ShouldSaveNewImageAndDeletePrevious()
        {
            // Arrange
            AboutSiteContent existing = AboutSiteContentDataBuilder.Create().WithImageUrl("/uploads/antiga.jpg");
            var image = new FileUploadDto(new MemoryStream(), "nova.jpg", "image/jpeg", 1024);
            var input = new UpsertAboutSiteInput("Cargo", new List<string>(), "Texto", new List<string>(), new List<string>(), image);
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

            _repoMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<AboutSiteContent> { existing });
            _fileStorageServiceMock.SaveAsync(image, Arg.Any<CancellationToken>()).Returns("/uploads/nova.jpg");

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.ImageUrl.ShouldBe("/uploads/nova.jpg");

            await _fileStorageServiceMock.Received(1).SaveAsync(image, Arg.Any<CancellationToken>());
            await _fileStorageServiceMock.Received(1).DeleteAsync("/uploads/antiga.jpg", Arg.Any<CancellationToken>());
            await _repoMock.Received(1).UpdateAsync(Arg.Is<AboutSiteContent>(c => c.ImageUrl == "/uploads/nova.jpg"), Arg.Any<CancellationToken>());
        }

        [Fact(DisplayName = "ERRO - Deve retornar Validation e não gravar nada quando os dados forem inválidos")]
        public async Task ExecuteAsync_WhenValidationFails_ShouldReturnValidationErrorsAndNotWrite()
        {
            // Arrange
            var input = new UpsertAboutSiteInput("", new List<string>(), "", new List<string>(), new List<string>(), null);

            var validationFailures = new List<ValidationFailure>
            {
                new(nameof(UpsertAboutSiteInput.Cargo), "O cargo é obrigatório.")
            };
            _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>()).Returns(new ValidationResult(validationFailures));

            // Act
            var result = await _sut.ExecuteAsync(input, CancellationToken.None);

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Validation);
            result.FirstError.Code.ShouldBe(nameof(UpsertAboutSiteInput.Cargo));

            await _repoMock.DidNotReceiveWithAnyArgs().GetAllAsync(default);
            await _fileStorageServiceMock.DidNotReceiveWithAnyArgs().SaveAsync(default!, default);
        }
    }
}
