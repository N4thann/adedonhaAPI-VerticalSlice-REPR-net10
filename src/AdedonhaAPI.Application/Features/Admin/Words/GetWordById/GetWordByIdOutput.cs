namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordById
{
    public record GetWordByIdOutput(string Id, string Name, string Slug, char InitialLetter, string? Description, IReadOnlyList<string> CategoryIds);
}
