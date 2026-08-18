using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWordCounts
{
    public record GetCatalogCategoryWordCountsInput() : IInput<ErrorOr<GetCatalogCategoryWordCountsOutput>>;
}
