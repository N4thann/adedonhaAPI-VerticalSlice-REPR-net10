using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.EditCategory;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de edicao de categoria (Admin).
    /// </summary>
    public class EditCategoryEndpoint : ICarterModule
    {
        public record EditCategoryRequest(string Name, string? Description);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/v1/admin/categories/{id}", async (
                string id,
                [FromBody] EditCategoryRequest request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new EditCategoryInput(id, request.Name, request.Description), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Categories")
            .WithName("EditCategory")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<EditCategoryOutput>(StatusCodes.Status200OK);
        }
    }
}
