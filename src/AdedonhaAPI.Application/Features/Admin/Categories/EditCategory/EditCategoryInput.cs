using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.EditCategory
{
    public record EditCategoryInput(string Id, string Name, string? Description) : IInput<ErrorOr<EditCategoryOutput>>;
}
