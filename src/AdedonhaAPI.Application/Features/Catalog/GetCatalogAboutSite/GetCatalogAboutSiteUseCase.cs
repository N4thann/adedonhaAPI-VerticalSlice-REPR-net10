using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogAboutSite
{
    /// <summary>Consulta pública o texto "Sobre o site" (bio do criador) — documento único e global.</summary>
    public class GetCatalogAboutSiteUseCase : IUseCase<GetCatalogAboutSiteInput, ErrorOr<GetCatalogAboutSiteOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogAboutSiteUseCase> _logger;

        public GetCatalogAboutSiteUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCatalogAboutSiteUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogAboutSiteOutput>> ExecuteAsync(GetCatalogAboutSiteInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Consulta do texto Sobre o site", _requestContext);

            var about = (await _unitOfWork.AboutSite.GetAllAsync(cancellationToken)).FirstOrDefault();

            _logger.LogEnd("Consulta do texto Sobre o site", _requestContext);

            return new GetCatalogAboutSiteOutput(
                about?.Cargo ?? string.Empty,
                about?.Formacoes ?? new List<string>(),
                about?.TextoGeral ?? string.Empty,
                about?.Tecnologias ?? new List<string>(),
                about?.Arquiteturas ?? new List<string>(),
                about?.ImageUrl);
        }
    }
}
