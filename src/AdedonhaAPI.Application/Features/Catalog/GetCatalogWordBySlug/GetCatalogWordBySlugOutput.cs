namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordBySlug
{
    public record CatalogWordCategorySummary(string Slug, string Name);

    public record GetCatalogWordBySlugOutput(string Name, string? Description, IReadOnlyList<CatalogWordCategorySummary> Categories);
}
