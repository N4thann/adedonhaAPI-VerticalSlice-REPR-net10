using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.DisassociateWordFromCategory
{
    /// <summary>
    /// Remove a associacao de uma palavra com uma categoria. Idempotente: desassociar quando
    /// ja nao ha associacao nao gera erro.
    /// </summary>
    public class DisassociateWordFromCategoryUseCase : IUseCase<DisassociateWordFromCategoryInput, ErrorOr<DisassociateWordFromCategoryOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<DisassociateWordFromCategoryUseCase> _logger;

        public DisassociateWordFromCategoryUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<DisassociateWordFromCategoryUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<DisassociateWordFromCategoryOutput>> ExecuteAsync(DisassociateWordFromCategoryInput input, CancellationToken cancellationToken)
        {
            var word = await _unitOfWork.Words.GetByIdAsync(input.WordId, cancellationToken);
            if (word is null || !word.IsActive)
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");

            var category = await _unitOfWork.Categories.GetByIdAsync(input.CategoryId, cancellationToken);
            if (category is null)
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");

            var removed = word.Categories.RemoveAll(c => c.CategoryId == input.CategoryId);
            if (removed > 0)
            {
                await _unitOfWork.Words.UpdateAsync(word, cancellationToken);
                _logger.LogInfo("Palavra desassociada de categoria", _requestContext, new() { ["WordId"] = word.Id, ["CategoryId"] = input.CategoryId });
            }

            return new DisassociateWordFromCategoryOutput(word.Id, word.Categories.Select(c => c.CategoryId).ToList());
        }
    }
}
