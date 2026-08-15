using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryById
{
    public record GetCategoryByIdInput(string Id) : IInput<ErrorOr<GetCategoryByIdOutput>>;
}
