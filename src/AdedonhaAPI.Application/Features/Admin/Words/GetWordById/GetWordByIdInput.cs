using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordById
{
    public record GetWordByIdInput(string Id) : IInput<ErrorOr<GetWordByIdOutput>>;
}
