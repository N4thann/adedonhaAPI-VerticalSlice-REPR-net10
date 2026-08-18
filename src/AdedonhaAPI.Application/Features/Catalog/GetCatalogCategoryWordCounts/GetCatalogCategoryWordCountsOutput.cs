namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWordCounts
{
    public record CatalogCategoryWordCount(string Name, string Slug, int WordCount);

    public record GetCatalogCategoryWordCountsOutput(IReadOnlyList<CatalogCategoryWordCount> Items);
}
