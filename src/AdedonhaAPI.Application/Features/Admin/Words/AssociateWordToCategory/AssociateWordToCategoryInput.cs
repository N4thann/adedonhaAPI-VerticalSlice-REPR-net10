using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.AssociateWordToCategory
{
    public record AssociateWordToCategoryInput(string WordId, string CategoryId) : IInput<ErrorOr<AssociateWordToCategoryOutput>>;
}
