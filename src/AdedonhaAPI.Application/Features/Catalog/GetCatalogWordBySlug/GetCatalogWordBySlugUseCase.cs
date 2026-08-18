using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogWordBySlug
{
    /// <summary>
    /// Busca os dados publicos de uma palavra ativa pelo Slug, incluindo as categorias
    /// as quais ela pertence.
    /// </summary>
    public class GetCatalogWordBySlugUseCase : IUseCase<GetCatalogWordBySlugInput, ErrorOr<GetCatalogWordBySlugOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogWordBySlugUseCase> _logger;

        public GetCatalogWordBySlugUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCatalogWordBySlugUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogWordBySlugOutput>> ExecuteAsync(GetCatalogWordBySlugInput input, CancellationToken cancellationToken)
        {
            var word = (await _unitOfWork.Words.FindAsync(w => w.Slug == input.Slug && w.IsActive, cancellationToken)).FirstOrDefault();
            if (word is null)
            {
                _logger.LogWarning("Palavra do catalogo nao encontrada", _requestContext, new() { ["Slug"] = input.Slug });
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");
            }

            var categories = word.Categories.Select(c => new CatalogWordCategorySummary(c.Slug, c.Name)).ToList();
            return new GetCatalogWordBySlugOutput(word.Name, word.Description, categories);
        }
    }
}
