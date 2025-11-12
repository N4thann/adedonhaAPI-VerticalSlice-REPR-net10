using Application.Interfaces;
using Carter;
using Carter.OpenApi;

namespace AdedonhaAPI.Features.Admin.Users.CreateAdmin
{
    public class SeedAdminUserEndpoint : CarterModule
    {
        public SeedAdminUserEndpoint()
        {
            WithTags("Admin - Auth");
            WithSummary("Seed Admin User");
            WithDescription("Creates a default admin user in the system.");
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/admin/auth/seed-admin",
                async (IMediator mediator, CancellationToken cancellation) =>
                {
                    var command = new SeedAdminUserCommand();
                    var result = await mediator.SendCommand<SeedAdminUserCommand, string>(command, cancellation);
                    return Results.Ok(result);
                })               
                .AllowAnonymous()
                .IncludeInOpenApi();
        }
    }
}
