using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.GetWordStats;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de estatisticas de palavras (total e multi-categoria) para o dashboard (Admin).
    /// </summary>
    public class GetWordStatsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/words/stats", async (
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetWordStatsInput(), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("GetWordStats")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetWordStatsOutput>(StatusCodes.Status200OK);
        }
    }
}
