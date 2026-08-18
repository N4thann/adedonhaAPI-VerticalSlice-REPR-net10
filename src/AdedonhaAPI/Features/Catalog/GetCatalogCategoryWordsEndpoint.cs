using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Catalog
{
    /// <summary>
    /// Endpoint publico de palavras de uma categoria, paginado e com filtro opcional de letra/busca.
    /// </summary>
    public class GetCatalogCategoryWordsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/catalog/categories/{slug}/words", async (
                string slug,
                IMediator mediator,
                int page,
                int pageSize,
                char? letter,
                string? search,
                int seed,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCatalogCategoryWordsInput(slug, page, pageSize, letter, search, seed), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Catalog")
            .WithName("GetCatalogCategoryWords")
            .AllowAnonymous()
            .Produces<GetCatalogCategoryWordsOutput>(StatusCodes.Status200OK);
        }
    }
}
