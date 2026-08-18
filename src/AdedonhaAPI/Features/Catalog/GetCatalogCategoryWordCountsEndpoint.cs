using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWordCounts;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico de contagem de palavras por categoria, para o grafico donut do mural.
    /// </summary>
    public class GetCatalogCategoryWordCountsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/categories/word-counts", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogCategoryWordCountsInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogCategoryWordCounts")
            .AllowAnonymous()
            .Produces<GetCatalogCategoryWordCountsOutput>(StatusCodes.Status200OK);
        }
    }
}
