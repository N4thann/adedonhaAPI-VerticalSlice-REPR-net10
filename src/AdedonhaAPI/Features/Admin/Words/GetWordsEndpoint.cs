using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.GetWords;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de listagem paginada de palavras (Admin).
    /// </summary>
    public class GetWordsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/words", async (
                IMediator mediator,
                int page,
                int pageSize,
                string? search,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetWordsInput(page, pageSize, search), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("GetWords")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetWordsOutput>(StatusCodes.Status200OK);
        }
    }
}
