using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories
{
    /// <summary>
    /// Lista categorias ativas com uma amostra aleatoria de ate 10 palavras ativas cada,
    /// recalculada a cada chamada.
    /// </summary>
    public class GetCatalogCategoriesUseCase : IUseCase<GetCatalogCategoriesInput, ErrorOr<GetCatalogCategoriesOutput>>
    {
        private const int SampleSize = 10;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogCategoriesUseCase> _logger;

        public GetCatalogCategoriesUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCatalogCategoriesUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogCategoriesOutput>> ExecuteAsync(GetCatalogCategoriesInput input, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Listando mural de categorias do catalogo", _requestContext);

            var categories = await _unitOfWork.Categories.FindAsync(c => c.IsActive, cancellationToken);

            var items = new List<CatalogCategorySummary>();
            foreach (var category in categories)
            {
                var categoryId = category.Id;
                var sampleWords = await _unitOfWork.Words.GetRandomSampleAsync(
                    w => w.IsActive && w.Categories.Any(c => c.CategoryId == categoryId), SampleSize, cancellationToken);

                items.Add(new CatalogCategorySummary(
                    category.Slug,
                    category.Name,
                    category.Description,
                    sampleWords.Select(w => new CatalogWordSummary(w.Name, w.Slug, w.Description)).ToList()));
            }

            return new GetCatalogCategoriesOutput(items);
        }
    }
}
