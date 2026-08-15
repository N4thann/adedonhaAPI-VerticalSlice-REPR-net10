using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico do mural de categorias do catalogo.
    /// </summary>
    public class GetCatalogCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/categories", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogCategoriesInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogCategories")
            .AllowAnonymous()
            .Produces<GetCatalogCategoriesOutput>(StatusCodes.Status200OK);
        }
    }
}
