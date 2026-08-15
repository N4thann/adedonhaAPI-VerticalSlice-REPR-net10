namespace AdedonhaAPI.Application.Features.Admin.Words.DisassociateWordFromCategory
{
    public record DisassociateWordFromCategoryOutput(string WordId, IReadOnlyList<string> CategoryIds);
}
