using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico de estatisticas de palavras (total e multi-categoria), para o mural do catalogo.
    /// </summary>
    public class GetCatalogWordStatsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/words/stats", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogWordStatsInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogWordStats")
            .AllowAnonymous()
            .Produces<GetCatalogWordStatsOutput>(StatusCodes.Status200OK);
        }
    }
}
