using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats
{
    /// <summary>
    /// Calcula o total publico de palavras ativas, para o mural do catalogo.
    /// </summary>
    public class GetCatalogWordStatsUseCase : IUseCase<GetCatalogWordStatsInput, ErrorOr<GetCatalogWordStatsOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogWordStatsUseCase> _logger;

        public GetCatalogWordStatsUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCatalogWordStatsUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogWordStatsOutput>> ExecuteAsync(GetCatalogWordStatsInput input, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Calculando total publico de palavras", _requestContext);

            var words = await _unitOfWork.Words.FindAsync(w => w.IsActive, cancellationToken);

            return new GetCatalogWordStatsOutput(words.Count());
        }
    }
}
