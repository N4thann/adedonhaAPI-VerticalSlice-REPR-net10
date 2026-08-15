using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.DeleteCategory;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de remocao (soft delete) de categoria (Admin).
    /// </summary>
    public class DeleteCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/v1/admin/categories/{id}", async (
                string id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new DeleteCategoryInput(id), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Categories")
            .WithName("DeleteCategory")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<DeleteCategoryOutput>(StatusCodes.Status200OK);
        }
    }
}
