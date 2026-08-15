using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.DeleteWord;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de remocao (soft delete) de palavra (Admin).
    /// </summary>
    public class DeleteWordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/v1/admin/words/{id}", async (
                string id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new DeleteWordInput(id), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("DeleteWord")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<DeleteWordOutput>(StatusCodes.Status200OK);
        }
    }
}
