namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryWordCounts
{
    public record CategoryWordCount(string Name, string Slug, int WordCount);

    public record GetCategoryWordCountsOutput(IReadOnlyList<CategoryWordCount> Items);
}
