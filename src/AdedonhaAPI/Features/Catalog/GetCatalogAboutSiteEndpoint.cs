using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogAboutSite;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico do texto "Sobre o site" (bio do criador).
    /// </summary>
    public class GetCatalogAboutSiteEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/about-site", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogAboutSiteInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogAboutSite")
            .AllowAnonymous()
            .Produces<GetCatalogAboutSiteOutput>(StatusCodes.Status200OK);
        }
    }
}
