using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.DeleteCategory
{
    public record DeleteCategoryInput(string Id) : IInput<ErrorOr<DeleteCategoryOutput>>;
}
