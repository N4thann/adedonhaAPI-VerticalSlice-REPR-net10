using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryBySlug
{
    /// <summary>
    /// Busca os dados publicos de uma categoria ativa pelo Slug.
    /// </summary>
    public class GetCatalogCategoryBySlugUseCase : IUseCase<GetCatalogCategoryBySlugInput, ErrorOr<GetCatalogCategoryBySlugOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogCategoryBySlugUseCase> _logger;

        public GetCatalogCategoryBySlugUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCatalogCategoryBySlugUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogCategoryBySlugOutput>> ExecuteAsync(GetCatalogCategoryBySlugInput input, CancellationToken cancellationToken)
        {
            var category = (await _unitOfWork.Categories.FindAsync(c => c.Slug == input.Slug && c.IsActive, cancellationToken)).FirstOrDefault();
            if (category is null)
            {
                _logger.LogWarning("Categoria do catalogo nao encontrada", _requestContext, new() { ["Slug"] = input.Slug });
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");
            }

            return new GetCatalogCategoryBySlugOutput(category.Name, category.Slug, category.Description);
        }
    }
}
