namespace AdedonhaAPI.Application.Features.Admin.Words.AssociateWordToCategory
{
    public record AssociateWordToCategoryOutput(string WordId, IReadOnlyList<string> CategoryIds);
}
