namespace AdedonhaAPI.Application.Features.Admin.Words.GetWords
{
    public record WordSummary(string Id, string Name, string Slug, char InitialLetter, string? Description, IReadOnlyList<string> CategoryNames);

    public record GetWordsOutput(IReadOnlyList<WordSummary> Items, long TotalCount, int Page, int PageSize);
}
