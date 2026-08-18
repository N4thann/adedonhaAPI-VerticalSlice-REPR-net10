using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats
{
    public record GetCatalogWordStatsInput() : IInput<ErrorOr<GetCatalogWordStatsOutput>>;
}
