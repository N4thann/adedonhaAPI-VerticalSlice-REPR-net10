using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryById
{
    /// <summary>
    /// Busca uma categoria ativa pelo Id.
    /// </summary>
    public class GetCategoryByIdUseCase : IUseCase<GetCategoryByIdInput, ErrorOr<GetCategoryByIdOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCategoryByIdUseCase> _logger;

        public GetCategoryByIdUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<GetCategoryByIdUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCategoryByIdOutput>> ExecuteAsync(GetCategoryByIdInput input, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(input.Id, cancellationToken);
            if (category is null || !category.IsActive)
            {
                _logger.LogWarning("Categoria não encontrada", _requestContext, new() { ["CategoryId"] = input.Id });
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");
            }

            return new GetCategoryByIdOutput(category.Id, category.Name, category.Slug, category.Description);
        }
    }
}
