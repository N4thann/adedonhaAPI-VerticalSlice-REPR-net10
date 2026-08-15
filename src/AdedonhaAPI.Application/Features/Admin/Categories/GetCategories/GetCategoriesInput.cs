using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategories
{
    public record GetCategoriesInput(int Page, int PageSize, string? Search) : IInput<ErrorOr<GetCategoriesOutput>>;
}
