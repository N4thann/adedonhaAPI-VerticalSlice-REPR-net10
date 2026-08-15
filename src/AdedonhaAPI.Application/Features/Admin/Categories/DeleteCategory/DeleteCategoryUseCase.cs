using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Categories.DeleteCategory
{
    /// <summary>
    /// Soft delete de categoria. Bloqueia se houver palavra ativa associada.
    /// </summary>
    public class DeleteCategoryUseCase : IUseCase<DeleteCategoryInput, ErrorOr<DeleteCategoryOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<DeleteCategoryUseCase> _logger;

        public DeleteCategoryUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<DeleteCategoryUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<DeleteCategoryOutput>> ExecuteAsync(DeleteCategoryInput input, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(input.Id, cancellationToken);
            if (category is null || !category.IsActive)
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");

            var associatedWords = await _unitOfWork.Words.FindAsync(
                w => w.IsActive && w.Categories.Any(c => c.CategoryId == category.Id), cancellationToken);

            var count = associatedWords.Count();
            if (count > 0)
                return Error.Conflict("Category.Conflict.HasAssociatedWords", $"Não é possível excluir: {count} palavra(s) associada(s) a esta categoria.");

            category.IsActive = false;
            await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);

            _logger.LogInfo("Categoria removida (soft delete)", _requestContext, new() { ["CategoryId"] = category.Id });

            return new DeleteCategoryOutput(category.Id);
        }
    }
}
