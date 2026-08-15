using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWords
{
    /// <summary>
    /// Lista palavras ativas, paginadas e opcionalmente filtradas por nome.
    /// </summary>
    public class GetWordsUseCase : IUseCase<GetWordsInput, ErrorOr<GetWordsOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetWordsInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<GetWordsUseCase> _logger;

        public GetWordsUseCase(
            IUnitOfWork unitOfWork,
            IValidator<GetWordsInput> validator,
            IRequestContext requestContext,
            ILogger<GetWordsUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<GetWordsOutput>> ExecuteAsync(GetWordsInput input, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            _logger.LogInfo("Listando palavras", _requestContext, new() { ["Page"] = input.Page, ["Search"] = input.Search });

            Expression<Func<Word, bool>> filter = string.IsNullOrWhiteSpace(input.Search)
                ? w => w.IsActive
                : w => w.IsActive && w.Name.ToLower().Contains(input.Search.ToLower());

            var paged = await _unitOfWork.Words.GetPagedAsync(filter, w => w.Name, ascending: true, input.Page, input.PageSize, cancellationToken);

            var items = paged.Items
                .Select(w => new WordSummary(w.Id, w.Name, w.Slug, w.InitialLetter, w.Description, w.Categories.Select(c => c.Name).ToList()))
                .ToList();

            return new GetWordsOutput(items, paged.TotalCount, paged.Page, paged.PageSize);
        }
    }
}
