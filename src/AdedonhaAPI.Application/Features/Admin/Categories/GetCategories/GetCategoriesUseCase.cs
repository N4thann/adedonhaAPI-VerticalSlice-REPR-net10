using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategories
{
    /// <summary>
    /// Lista categorias ativas, paginadas e opcionalmente filtradas por nome.
    /// </summary>
    public class GetCategoriesUseCase : IUseCase<GetCategoriesInput, ErrorOr<GetCategoriesOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetCategoriesInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetCategoriesUseCase> _logger;

        public GetCategoriesUseCase(
            IUnitOfWork unitOfWork,
            IValidator<GetCategoriesInput> validator,
            IRequestContext requestContext,
            ILogger<GetCategoriesUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetCategoriesOutput>> ExecuteAsync(GetCategoriesInput input, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            _logger.LogInfo("Listando categorias", _requestContext, new() { ["Page"] = input.Page, ["Search"] = input.Search });

            Expression<Func<Category, bool>> filter = string.IsNullOrWhiteSpace(input.Search)
                ? c => c.IsActive
                : c => c.IsActive && c.Name.ToLower().Contains(input.Search.ToLower());

            var paged = await _unitOfWork.Categories.GetPagedAsync(filter, c => c.Name, ascending: true, input.Page, input.PageSize, cancellationToken);

            var items = paged.Items.Select(c => new CategorySummary(c.Id, c.Name, c.Slug, c.Description)).ToList();

            return new GetCategoriesOutput(items, paged.TotalCount, paged.Page, paged.PageSize);
        }
    }
}
