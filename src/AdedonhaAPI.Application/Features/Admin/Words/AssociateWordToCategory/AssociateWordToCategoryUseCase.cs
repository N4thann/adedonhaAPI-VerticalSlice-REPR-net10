using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.AssociateWordToCategory
{
    /// <summary>
    /// Associa uma palavra a uma categoria. Idempotente: associar duas vezes nao gera erro nem duplicata.
    /// </summary>
    public class AssociateWordToCategoryUseCase : IUseCase<AssociateWordToCategoryInput, ErrorOr<AssociateWordToCategoryOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<AssociateWordToCategoryUseCase> _logger;

        public AssociateWordToCategoryUseCase(IUnitOfWork unitOfWork, IRequestContext requestContext, ILogger<AssociateWordToCategoryUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<AssociateWordToCategoryOutput>> ExecuteAsync(AssociateWordToCategoryInput input, CancellationToken cancellationToken)
        {
            var word = await _unitOfWork.Words.GetByIdAsync(input.WordId, cancellationToken);
            if (word is null || !word.IsActive)
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");

            var category = await _unitOfWork.Categories.GetByIdAsync(input.CategoryId, cancellationToken);
            if (category is null || !category.IsActive)
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");

            if (!word.Categories.Any(c => c.CategoryId == category.Id))
            {
                word.Categories.Add(new Word.CategoryInfo { CategoryId = category.Id, Slug = category.Slug, Name = category.Name });
                await _unitOfWork.Words.UpdateAsync(word, cancellationToken);

                _logger.LogInfo("Palavra associada a categoria", _requestContext, new() { ["WordId"] = word.Id, ["CategoryId"] = category.Id });
            }

            return new AssociateWordToCategoryOutput(word.Id, word.Categories.Select(c => c.CategoryId).ToList());
        }
    }
}
