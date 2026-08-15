using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Application.Features.Catalog.GetCatalogCategories;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords
{
    /// <summary>
    /// Lista palavras ativas de uma categoria (por Slug), paginadas, com filtro opcional
    /// de letra inicial e busca por nome.
    /// </summary>
    public class GetCatalogCategoryWordsUseCase : IUseCase<GetCatalogCategoryWordsInput, ErrorOr<GetCatalogCategoryWordsOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetCatalogCategoryWordsInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCatalogCategoryWordsUseCase> _logger;

        public GetCatalogCategoryWordsUseCase(
            IUnitOfWork unitOfWork,
            IValidator<GetCatalogCategoryWordsInput> validator,
            IRequestContext requestContext,
            ILogger<GetCatalogCategoryWordsUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCatalogCategoryWordsOutput>> ExecuteAsync(GetCatalogCategoryWordsInput input, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var category = (await _unitOfWork.Categories.FindAsync(c => c.Slug == input.CategorySlug && c.IsActive, cancellationToken)).FirstOrDefault();
            if (category is null)
                return Error.NotFound("Category.NotFound", "Categoria não encontrada.");

            _logger.LogInfo("Listando palavras da categoria", _requestContext, new()
            {
                ["CategorySlug"] = input.CategorySlug,
                ["Letter"] = input.Letter,
                ["Search"] = input.Search
            });

            var categoryId = category.Id;
            var letter = input.Letter;
            var search = input.Search?.ToLower();

            Expression<Func<Word, bool>> filter = w =>
                w.IsActive &&
                w.Categories.Any(c => c.CategoryId == categoryId) &&
                (letter == null || w.InitialLetter == letter) &&
                (search == null || w.Name.ToLower().Contains(search));

            var paged = await _unitOfWork.Words.GetPagedAsync(filter, w => w.Name, ascending: true, input.Page, input.PageSize, cancellationToken);

            var items = paged.Items.Select(w => new CatalogWordSummary(w.Name, w.Slug, w.Description)).ToList();

            return new GetCatalogCategoryWordsOutput(items, paged.TotalCount, paged.Page, paged.PageSize);
        }
    }
}
