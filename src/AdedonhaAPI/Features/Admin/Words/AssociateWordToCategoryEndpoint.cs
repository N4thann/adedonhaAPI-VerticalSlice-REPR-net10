using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.AssociateWordToCategory;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de associacao de palavra a categoria (Admin).
    /// </summary>
    public class AssociateWordToCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/admin/words/{wordId}/categories/{categoryId}", async (
                string wordId,
                string categoryId,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new AssociateWordToCategoryInput(wordId, categoryId), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("AssociateWordToCategory")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<AssociateWordToCategoryOutput>(StatusCodes.Status200OK);
        }
    }
}
