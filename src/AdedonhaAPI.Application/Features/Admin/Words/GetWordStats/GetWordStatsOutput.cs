namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordStats
{
    public record WordCategoryCount(string Name, string Slug, int CategoryCount);

    public record GetWordStatsOutput(int TotalWords, IReadOnlyList<WordCategoryCount> WordsInMultipleCategories);
}
