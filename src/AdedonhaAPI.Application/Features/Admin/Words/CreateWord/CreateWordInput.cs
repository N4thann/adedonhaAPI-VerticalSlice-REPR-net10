using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.CreateWord
{
    public record CreateWordInput(string Name, string? Description, List<string>? CategoryIds) : IInput<ErrorOr<CreateWordOutput>>;
}
