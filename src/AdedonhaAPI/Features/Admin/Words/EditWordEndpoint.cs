using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.EditWord;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de edicao de palavra (Admin).
    /// </summary>
    public class EditWordEndpoint : ICarterModule
    {
        public record EditWordRequest(string Name, string? Description);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/v1/admin/words/{id}", async (
                string id,
                [FromBody] EditWordRequest request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new EditWordInput(id, request.Name, request.Description), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("EditWord")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<EditWordOutput>(StatusCodes.Status200OK);
        }
    }
}
