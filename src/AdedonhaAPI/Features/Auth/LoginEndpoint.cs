using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Auth.Login;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Auth
{
    /// <summary>
    /// Endpoint de autenticacao (emissao de JWT).
    /// </summary>
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/auth/login", async (
                [FromBody] LoginInput input,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(input, ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Auth")
            .WithName("Login")
            .AllowAnonymous()
            .RequireRateLimiting("fixedwindow")
            .Produces<LoginOutput>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }
}
