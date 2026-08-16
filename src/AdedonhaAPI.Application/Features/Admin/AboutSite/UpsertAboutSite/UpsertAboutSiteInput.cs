using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Common.Storage;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite
{
    /// <summary>
    /// <paramref name="Image"/> é opcional: quando omitido, a imagem já cadastrada (se houver) é
    /// mantida; quando enviado, substitui a anterior (que é apagada do armazenamento).
    /// </summary>
    public record UpsertAboutSiteInput(
        string Cargo,
        List<string> Formacoes,
        string TextoGeral,
        List<string> Tecnologias,
        List<string> Arquiteturas,
        FileUploadDto? Image) : IInput<ErrorOr<UpsertAboutSiteOutput>>;
}
