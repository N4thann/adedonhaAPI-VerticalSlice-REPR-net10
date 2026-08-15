using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Categories.GetCategories;
using AdedonhaAPI.Extensions;
using Carter;

namespace AdedonhaAPI.Features.Admin.Categories
{
    /// <summary>
    /// Endpoint de listagem paginada de categorias (Admin).
    /// </summary>
    public class GetCategoriesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/admin/categories", async (
                IMediator mediator,
                int page,
                int pageSize,
                string? search,
                CancellationToken ct) =>
            {
                var result = await mediator.SendAsync(new GetCategoriesInput(page, pageSize, search), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Categories")
            .WithName("GetCategories")
            .RequireAuthorization()
            .RequireAdmin()
            .Produces<GetCategoriesOutput>(StatusCodes.Status200OK);
        }
    }
}
