using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordBySlug
{
    public record GetCatalogWordBySlugInput(string Slug) : IInput<ErrorOr<GetCatalogWordBySlugOutput>>;
}
