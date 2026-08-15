using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de criacao de categoria (Admin).
    /// </summary>
    public class CreateCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/admin/categories", async (
                [FromBody] CreateCategoryInput input,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(input, ct);
                return result.MatchResponse(output => Results.Created($"/api/v1/admin/categories/{output.Id}", output));
            })
            .WithTags("Admin - Categories")
            .WithName("CreateCategory")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<CreateCategoryOutput>(StatusCodes.Status201Created);
        }
    }
}
