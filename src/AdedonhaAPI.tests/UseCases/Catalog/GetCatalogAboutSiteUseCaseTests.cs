using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogAboutSite;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.tests.DataBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace AdedonhaAPI.tests.UseCases.Catalog
{
    public class GetCatalogAboutSiteUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly IRepository<AboutSiteContent> _repoMock;
        private readonly IRequestContext _requestContextMock;
        private readonly ILogger<GetCatalogAboutSiteUseCase> _loggerMock;
        private readonly GetCatalogAboutSiteUseCase _sut;

        public GetCatalogAboutSiteUseCaseTests()
        {
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _repoMock = Substitute.For<IRepository<AboutSiteContent>>();
            _requestContextMock = Substitute.For<IRequestContext>();
            _loggerMock = Substitute.For<ILogger<GetCatalogAboutSiteUseCase>>();

            _unitOfWorkMock.AboutSite.Returns(_repoMock);

            _sut = new GetCatalogAboutSiteUseCase(_unitOfWorkMock, _requestContextMock, _loggerMock);
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar os dados cadastrados quando já existir conteúdo")]
        public async Task ExecuteAsync_WhenContentExists_ShouldReturnStoredData()
        {
            // Arrange
            AboutSiteContent content = AboutSiteContentDataBuilder.Create()
                .WithCargo("Engenheiro de Software")
                .WithFormacoes(new List<string> { "Ciência da Computação", "Pós em DevOps" })
                .WithTextoGeral("Texto geral")
                .WithTecnologias(new List<string> { ".NET", "MongoDB" })
                .WithArquiteturas(new List<string> { "Vertical Slice", "REPR" })
                .WithImageUrl("/uploads/foto.jpg");

            _repoMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<AboutSiteContent> { content });

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogAboutSiteInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Cargo.ShouldBe("Engenheiro de Software");
            result.Value.Formacoes.Count.ShouldBe(2);
            result.Value.TextoGeral.ShouldBe("Texto geral");
            result.Value.Tecnologias.ShouldBe(new List<string> { ".NET", "MongoDB" });
            result.Value.Arquiteturas.ShouldBe(new List<string> { "Vertical Slice", "REPR" });
            result.Value.ImageUrl.ShouldBe("/uploads/foto.jpg");
        }

        [Fact(DisplayName = "SUCESSO - Deve retornar campos vazios quando nada tiver sido cadastrado ainda")]
        public async Task ExecuteAsync_WhenNoContentExists_ShouldReturnEmptyFields()
        {
            // Arrange
            _repoMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<AboutSiteContent>());

            // Act
            var result = await _sut.ExecuteAsync(new GetCatalogAboutSiteInput(), CancellationToken.None);

            // Assert
            result.IsError.ShouldBeFalse();
            result.Value.Cargo.ShouldBe(string.Empty);
            result.Value.Formacoes.ShouldBeEmpty();
            result.Value.TextoGeral.ShouldBe(string.Empty);
            result.Value.Tecnologias.ShouldBeEmpty();
            result.Value.Arquiteturas.ShouldBeEmpty();
            result.Value.ImageUrl.ShouldBeNull();
        }
    }
}
