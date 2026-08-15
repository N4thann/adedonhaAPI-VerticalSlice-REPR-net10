using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.DisassociateWordFromCategory
{
    public record DisassociateWordFromCategoryInput(string WordId, string CategoryId) : IInput<ErrorOr<DisassociateWordFromCategoryOutput>>;
}
