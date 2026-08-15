using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.DeleteWord
{
    public record DeleteWordInput(string Id) : IInput<ErrorOr<DeleteWordOutput>>;
}
