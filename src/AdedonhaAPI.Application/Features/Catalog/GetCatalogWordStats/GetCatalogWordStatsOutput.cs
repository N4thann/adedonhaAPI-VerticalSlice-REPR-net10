namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats
{
    public record CatalogWordCategoryCount(string Name, string Slug, int CategoryCount);

    public record GetCatalogWordStatsOutput(int TotalWords, IReadOnlyList<CatalogWordCategoryCount> WordsInMultipleCategories);
}
