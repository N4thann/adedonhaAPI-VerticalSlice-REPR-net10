using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory
{
    /// <summary>
    /// Cria uma nova categoria, gerando o Slug a partir do Name.
    /// </summary>
    public class CreateCategoryUseCase : IUseCase<CreateCategoryInput, ErrorOr<CreateCategoryOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCategoryInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<CreateCategoryUseCase> _logger;

        public CreateCategoryUseCase(
            IUnitOfWork unitOfWork,
            IValidator<CreateCategoryInput> validator,
            IRequestContext requestContext,
            ILogger<CreateCategoryUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateCategoryOutput>> ExecuteAsync(CreateCategoryInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Criando categoria", _requestContext, new() { ["Name"] = input.Name });

            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var slug = SlugGenerator.Generate(input.Name);
            var existing = await _unitOfWork.Categories.FindAsync(c => c.Slug == slug, cancellationToken);
            if (existing.Any())
                return Error.Conflict("Category.Conflict.NameAlreadyExists", $"Já existe uma categoria com o nome '{input.Name}'.");

            var category = new Category
            {
                Name = input.Name,
                Slug = slug,
                Description = input.Description
            };

            await _unitOfWork.Categories.AddAsync(category, cancellationToken);

            _logger.LogEnd("Criando categoria", _requestContext, new() { ["CategoryId"] = category.Id });

            return new CreateCategoryOutput(category.Id, category.Name, category.Slug, category.Description);
        }
    }
}
