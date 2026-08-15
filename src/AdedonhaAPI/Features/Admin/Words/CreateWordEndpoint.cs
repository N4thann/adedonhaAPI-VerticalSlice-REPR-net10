using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.CreateWord;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de criacao de palavra (Admin).
    /// </summary>
    public class CreateWordEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/admin/words", async (
                [FromBody] CreateWordInput input,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(input, ct);
                return result.MatchResponse(output => Results.Created($"/api/v1/admin/words/{output.Id}", output));
            })
            .WithTags("Admin - Words")
            .WithName("CreateWord")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<CreateWordOutput>(StatusCodes.Status201Created);
        }
    }
}
