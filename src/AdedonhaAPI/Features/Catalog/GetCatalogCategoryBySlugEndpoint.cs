using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryBySlug;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico de detalhe de categoria por Slug.
    /// </summary>
    public class GetCatalogCategoryBySlugEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/categories/{slug}", async (
                string slug,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogCategoryBySlugInput(slug), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogCategoryBySlug")
            .AllowAnonymous()
            .Produces<GetCatalogCategoryBySlugOutput>(StatusCodes.Status200OK);
        }
    }
}
