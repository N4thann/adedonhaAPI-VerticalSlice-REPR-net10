using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryBySlug
{
    public record GetCatalogCategoryBySlugInput(string Slug) : IInput<ErrorOr<GetCatalogCategoryBySlugOutput>>;
}
