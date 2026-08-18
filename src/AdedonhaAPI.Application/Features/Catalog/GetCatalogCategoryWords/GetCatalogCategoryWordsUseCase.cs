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
    /// Lista palavras ativas de uma categoria (por Slug), paginadas em ordem embaralhada
    /// deterministicamente por seed, com filtro opcional de letra inicial e busca por nome.
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

            _logger.LogInfo("Listando palavras da categoria (ordem embaralhada)", _requestContext, new()
            {
                ["CategorySlug"] = input.CategorySlug,
                ["Letter"] = input.Letter,
                ["Search"] = input.Search,
                ["Seed"] = input.Seed
            });

            var categoryId = category.Id;
            var letter = input.Letter;
            var search = input.Search?.ToLower();

            Expression<Func<Word, bool>> filter = w =>
                w.IsActive &&
                w.Categories.Any(c => c.CategoryId == categoryId) &&
                (letter == null || w.InitialLetter == letter) &&
                (search == null || w.Name.ToLower().Contains(search));

            var matching = (await _unitOfWork.Words.FindAsync(filter, cancellationToken)).ToList();
            var shuffled = ShuffleDeterministic(matching, input.Seed);
            var pageItems = shuffled.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToList();

            var items = pageItems.Select(w => new CatalogWordSummary(w.Name, w.Slug, w.Description)).ToList();

            return new GetCatalogCategoryWordsOutput(items, matching.Count, input.Page, input.PageSize);
        }

        private static List<Word> ShuffleDeterministic(List<Word> source, int seed)
        {
            var list = new List<Word>(source);
            var random = new Random(seed);
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }
    }
}
