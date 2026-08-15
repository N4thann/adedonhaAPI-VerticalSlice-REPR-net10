namespace AdedonhaAPI.Application.Features.Admin.Words.CreateWord
{
    public record CreateWordOutput(string Id, string Name, string Slug, char InitialLetter, string? Description, List<string> CategoryIds);
}
