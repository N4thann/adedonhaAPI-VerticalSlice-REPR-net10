using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Categories.EditCategory
{
    /// <summary>
    /// Atualiza Name/Description de uma categoria. Quando o Name muda, cascateia o novo Slug/Name
    /// para os registros desnormalizados Word.Categories[] de toda palavra associada.
    /// </summary>
    public class EditCategoryUseCase : IUseCase<EditCategoryInput, ErrorOr<EditCategoryOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<EditCategoryInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<EditCategoryUseCase> _logger;

        public EditCategoryUseCase(
            IUnitOfWork unitOfWork,
            IValidator<EditCategoryInput> validator,
            IRequestContext requestContext,
            ILogger<EditCategoryUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<EditCategoryOutput>> ExecuteAsync(EditCategoryInput input, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var category = await _unitOfWork.Categories.GetByIdAsync(input.Id, cancellationToken);
            if (category is null || !category.IsActive)
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");

            var nameChanged = !string.Equals(category.Name, input.Name, StringComparison.OrdinalIgnoreCase);
            var newSlug = SlugGenerator.Generate(input.Name);

            if (nameChanged)
            {
                var conflicting = await _unitOfWork.Categories.FindAsync(c => c.Slug == newSlug && c.Id != category.Id, cancellationToken);
                if (conflicting.Any())
                    return Error.Conflict("Category.Conflict.NameAlreadyExists", $"Já existe uma categoria com o nome '{input.Name}'.");
            }

            category.Name = input.Name;
            category.Slug = newSlug;
            category.Description = input.Description;
            await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);

            if (nameChanged)
            {
                _logger.LogInfo("Cascateando renomeacao de categoria para palavras associadas", _requestContext, new() { ["CategoryId"] = category.Id });

                var affectedWords = await _unitOfWork.Words.FindAsync(w => w.Categories.Any(c => c.CategoryId == category.Id), cancellationToken);
                foreach (var word in affectedWords)
                {
                    var categoryInfo = word.Categories.First(c => c.CategoryId == category.Id);
                    categoryInfo.Slug = category.Slug;
                    categoryInfo.Name = category.Name;
                    await _unitOfWork.Words.UpdateAsync(word, cancellationToken);
                }
            }

            return new EditCategoryOutput(category.Id, category.Name, category.Slug, category.Description);
        }
    }
}
