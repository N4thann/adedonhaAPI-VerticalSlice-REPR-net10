namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategories
{
    public record CategorySummary(string Id, string Name, string Slug, string? Description);

    public record GetCategoriesOutput(IReadOnlyList<CategorySummary> Items, long TotalCount, int Page, int PageSize);
}
