using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.EditWord
{
    public record EditWordInput(string Id, string Name, string? Description) : IInput<ErrorOr<EditWordOutput>>;
}
