using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory
{
    public record CreateCategoryInput(string Name, string? Description) : IInput<ErrorOr<CreateCategoryOutput>>;
}
