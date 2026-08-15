using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.DisassociateWordFromCategory;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de desassociacao de palavra de categoria (Admin).
    /// </summary>
    public class DisassociateWordFromCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/v1/admin/words/{wordId}/categories/{categoryId}", async (
                string wordId,
                string categoryId,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new DisassociateWordFromCategoryInput(wordId, categoryId), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("DisassociateWordFromCategory")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<DisassociateWordFromCategoryOutput>(StatusCodes.Status200OK);
        }
    }
}
