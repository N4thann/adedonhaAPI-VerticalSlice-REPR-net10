using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryById;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de busca de categoria por Id (Admin).
    /// </summary>
    public class GetCategoryByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/categories/{id}", async (
                string id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCategoryByIdInput(id), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Categories")
            .WithName("GetCategoryById")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetCategoryByIdOutput>(StatusCodes.Status200OK);
        }
    }
}
