namespace AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite
{
    public record UpsertAboutSiteOutput(
        string Cargo,
        IReadOnlyList<string> Formacoes,
        string TextoGeral,
        IReadOnlyList<string> Tecnologias,
        IReadOnlyList<string> Arquiteturas,
        string? ImageUrl);
}
