using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordStats
{
    /// <summary>
    /// Calcula estatisticas publicas de palavras: total ativo e as que estao associadas a mais de
    /// uma categoria, para o mural publico do catalogo.
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
            _logger.LogInfo("Calculando estatisticas publicas de palavras", _requestContext);

            var words = (await _unitOfWork.Words.FindAsync(w => w.IsActive, cancellationToken)).ToList();

            var multiCategory = words
                .Where(w => w.Categories.Count > 1)
                .Select(w => new CatalogWordCategoryCount(w.Name, w.Slug, w.Categories.Count))
                .OrderByDescending(w => w.CategoryCount)
                .ToList();

            return new GetCatalogWordStatsOutput(words.Count, multiCategory);
        }
    }
}
