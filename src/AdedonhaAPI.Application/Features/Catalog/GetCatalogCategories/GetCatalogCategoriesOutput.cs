namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories
{
    public record CatalogWordSummary(string Name, string Slug, string? Description);

    public record CatalogCategorySummary(string Slug, string Name, string? Description, IReadOnlyList<CatalogWordSummary> SampleWords);

    public record GetCatalogCategoriesOutput(IReadOnlyList<CatalogCategorySummary> Categories);
}
