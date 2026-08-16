namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogAboutSite
{
    /// <summary>Texto "Sobre o site" (bio do criador). Campos vêm vazios/nulos se o admin ainda não cadastrou nada.</summary>
    public record GetCatalogAboutSiteOutput(
        string Cargo,
        IReadOnlyList<string> Formacoes,
        string TextoGeral,
        IReadOnlyList<string> Tecnologias,
        IReadOnlyList<string> Arquiteturas,
        string? ImageUrl);
}
