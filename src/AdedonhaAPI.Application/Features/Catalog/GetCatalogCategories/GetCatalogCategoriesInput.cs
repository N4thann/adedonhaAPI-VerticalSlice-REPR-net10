using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories
{
    public record GetCatalogCategoriesInput() : IInput<ErrorOr<GetCatalogCategoriesOutput>>;
}
