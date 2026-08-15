using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.GetWordById;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de busca de palavra por Id (Admin).
    /// </summary>
    public class GetWordByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/words/{id}", async (
                string id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetWordByIdInput(id), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("GetWordById")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetWordByIdOutput>(StatusCodes.Status200OK);
        }
    }
}
