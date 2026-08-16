using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Common.Storage;
using AdedonhaAPI.Application.Features.Admin.AboutSite.UpsertAboutSite;
using AdedonhaAPI.Extensions;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AdedonhaAPI.Features.Admin.AboutSite
{
    /// <summary>
    /// Corpo do upsert de "Sobre o site" (multipart/form-data). <see cref="Image"/> é opcional —
    /// quando omitida numa atualização, a foto já cadastrada é mantida. <see cref="Formacoes"/>,
    /// <see cref="Tecnologias"/> e <see cref="Arquiteturas"/> são enviados como múltiplos campos
    /// de formulário com o mesmo nome.
    /// </summary>
    public class UpsertAboutSiteRequest
    {
        public string Cargo { get; set; } = string.Empty;
        public List<string> Formacoes { get; set; } = new();
        public string TextoGeral { get; set; } = string.Empty;
        public List<string> Tecnologias { get; set; } = new();
        public List<string> Arquiteturas { get; set; } = new();
        public IFormFile? Image { get; set; }
    }

    /// <summary>Endpoint (restrito a administradores) que cria ou atualiza o texto "Sobre o site", com upload opcional da foto.</summary>
    public class UpsertAboutSiteEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/v1/admin/about-site", async (
                [FromForm] UpsertAboutSiteRequest request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                FileUploadDto? image = request.Image is null
                    ? null
                    : new FileUploadDto(request.Image.OpenReadStream(), request.Image.FileName, request.Image.ContentType, request.Image.Length);

                var input = new UpsertAboutSiteInput(
                    request.Cargo,
                    request.Formacoes,
                    request.TextoGeral,
                    request.Tecnologias,
                    request.Arquiteturas,
                    image);
                var result = await mediator.SendAsync(input, ct);

                return result.MatchResponse(output => Results.Ok(output));
            })
            .WithTags("Admin - AboutSite")
            .WithName("UpsertAboutSite")
            .RequireAuthorization()
            .RequireAdmin()
            // Endpoints com [FromForm] recebem metadado de anti-forgery automaticamente (proteção
            // contra CSRF pensada para autenticação por cookie). Esta API usa só JWT Bearer — sem
            // cookie, o navegador não anexa Authorization sozinho em request cross-site, então
            // CSRF não se aplica e não há middleware de anti-forgery configurado (nem precisa).
            .DisableAntiforgery()
            .Produces<UpsertAboutSiteOutput>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
