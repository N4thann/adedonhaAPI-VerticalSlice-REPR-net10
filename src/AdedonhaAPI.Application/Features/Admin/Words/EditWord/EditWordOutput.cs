namespace AdedonhaAPI.Application.Features.Admin.Words.EditWord
{
    public record EditWordOutput(string Id, string Name, string Slug, char InitialLetter, string? Description);
}
