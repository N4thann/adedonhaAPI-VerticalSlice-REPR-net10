using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords
{
    public record GetCatalogCategoryWordsOutput(IReadOnlyList<CatalogWordSummary> Items, long TotalCount, int Page, int PageSize);
}
