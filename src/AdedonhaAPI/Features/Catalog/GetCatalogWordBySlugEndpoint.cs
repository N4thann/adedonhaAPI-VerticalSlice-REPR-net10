using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogWordBySlug;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico de detalhe de palavra por Slug.
    /// </summary>
    public class GetCatalogWordBySlugEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/words/{slug}", async (
                string slug,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogWordBySlugInput(slug), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogWordBySlug")
            .AllowAnonymous()
            .Produces<GetCatalogWordBySlugOutput>(StatusCodes.Status200OK);
        }
    }
}
