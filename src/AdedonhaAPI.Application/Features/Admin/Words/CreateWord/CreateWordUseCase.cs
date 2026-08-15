using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Entities;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.CreateWord
{
    /// <summary>
    /// Cria uma nova palavra, gerando Slug/InitialLetter a partir do Name, opcionalmente ja associada a categorias.
    /// </summary>
    public class CreateWordUseCase : IUseCase<CreateWordInput, ErrorOr<CreateWordOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateWordInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<CreateWordUseCase> _logger;

        public CreateWordUseCase(
            IUnitOfWork unitOfWork,
            IValidator<CreateWordInput> validator,
            IRequestContext requestContext,
            ILogger<CreateWordUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateWordOutput>> ExecuteAsync(CreateWordInput input, CancellationToken cancellationToken)
        {
            _logger.LogBegin("Criando palavra", _requestContext, new() { ["Name"] = input.Name });

            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var slug = SlugGenerator.Generate(input.Name);
            var existing = await _unitOfWork.Words.FindAsync(w => w.Slug == slug, cancellationToken);
            if (existing.Any())
                return Error.Conflict("Word.Conflict.NameAlreadyExists", $"Já existe uma palavra com o nome '{input.Name}'.");

            var categories = new List<Word.CategoryInfo>();
            foreach (var categoryId in input.CategoryIds ?? new List<string>())
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
                if (category is null || !category.IsActive)
                    return Error.NotFound("Category.NotFound", $"Categoria '{categoryId}' não encontrada.");

                categories.Add(new Word.CategoryInfo { CategoryId = category.Id, Slug = category.Slug, Name = category.Name });
            }

            var word = new Word
            {
                Name = input.Name,
                Slug = slug,
                InitialLetter = SlugGenerator.GetInitialLetter(input.Name),
                Description = input.Description,
                Categories = categories
            };

            await _unitOfWork.Words.AddAsync(word, cancellationToken);

            _logger.LogEnd("Criando palavra", _requestContext, new() { ["WordId"] = word.Id });

            return new CreateWordOutput(word.Id, word.Name, word.Slug, word.InitialLetter, word.Description, categories.Select(c => c.CategoryId).ToList());
        }
    }
}
