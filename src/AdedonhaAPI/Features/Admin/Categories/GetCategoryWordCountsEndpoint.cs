using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryWordCounts;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de contagem de palavras por categoria, para o grafico donut do dashboard (Admin).
    /// </summary>
    public class GetCategoryWordCountsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/categories/word-counts", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCategoryWordCountsInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Categories")
            .WithName("GetCategoryWordCounts")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetCategoryWordCountsOutput>(StatusCodes.Status200OK);
        }
    }
}
