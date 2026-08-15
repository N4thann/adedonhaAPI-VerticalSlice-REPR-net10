using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Http;

namespace AdedonhaAPI.Features.Admin.Words
{
    /// <summary>
    /// Endpoint de upload em massa de palavras via CSV (Admin).
    /// </summary>
    public class BulkUploadWordsCsvEndpoint : ICarterModule
    {
        private const long MaxFileSizeBytes = 5_000_000;

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/admin/words/bulk-upload", async (
                IFormFile file,
                IMediator mediator,
                CancellationToken ct) =>
            {
                if (file.Length == 0)
                    return Results.BadRequest("Arquivo vazio.");

                if (file.Length > MaxFileSizeBytes)
                    return Results.BadRequest("Arquivo excede o tamanho máximo permitido (5 MB).");

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return Results.BadRequest("Formato de arquivo inválido. Envie um arquivo .csv.");

                var lines = new List<string>();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync(ct)) != null)
                        lines.Add(line);
                }

                var result = await mediator.SendAsync(new BulkUploadWordsCsvInput(lines), ct);
                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - Words")
            .WithName("BulkUploadWordsCsv")
            .RequireAuthorization()
            .RequireAdmin()
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<BulkUploadWordsCsvOutput>(StatusCodes.Status200OK);
        }
    }
}
