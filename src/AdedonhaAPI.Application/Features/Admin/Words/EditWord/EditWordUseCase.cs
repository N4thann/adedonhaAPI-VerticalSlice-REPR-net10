using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Logging;
using AdedonhaAPI.Application.Common.Mediator;
using AdedonhaAPI.Domain.Common;
using AdedonhaAPI.Domain.Interfaces;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AdedonhaAPI.Application.Features.Admin.Words.EditWord
{
    /// <summary>
    /// Atualiza Name/Description de uma palavra, regenerando Slug/InitialLetter quando o Name muda.
    /// Nunca altera Categories[] — isso e responsabilidade exclusiva dos UseCases de associacao.
    /// </summary>
    public class EditWordUseCase : IUseCase<EditWordInput, ErrorOr<EditWordOutput>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<EditWordInput> _validator;
        private readonly IRequestContext _requestContext;
        private readonly ILogger<EditWordUseCase> _logger;

        public EditWordUseCase(
            IUnitOfWork unitOfWork,
            IValidator<EditWordInput> validator,
            IRequestContext requestContext,
            ILogger<EditWordUseCase> logger)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _requestContext = requestContext;
            _logger = logger;
        }

        public async Task<ErrorOr<EditWordOutput>> ExecuteAsync(EditWordInput input, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.Errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage)).ToList();

            var word = await _unitOfWork.Words.GetByIdAsync(input.Id, cancellationToken);
            if (word is null || !word.IsActive)
                return Error.NotFound("Word.NotFound", "Palavra não encontrada.");

            var nameChanged = !string.Equals(word.Name, input.Name, StringComparison.OrdinalIgnoreCase);
            var newSlug = SlugGenerator.Generate(input.Name);

            if (nameChanged)
            {
                var conflicting = await _unitOfWork.Words.FindAsync(w => w.Slug == newSlug && w.Id != word.Id, cancellationToken);
                if (conflicting.Any())
                    return Error.Conflict("Word.Conflict.NameAlreadyExists", $"Já existe uma palavra com o nome '{input.Name}'.");
            }

            word.Name = input.Name;
            word.Slug = newSlug;
            word.InitialLetter = SlugGenerator.GetInitialLetter(input.Name);
            word.Description = input.Description;

            await _unitOfWork.Words.UpdateAsync(word, cancellationToken);

            _logger.LogInfo("Palavra atualizada", _requestContext, new() { ["WordId"] = word.Id });

            return new EditWordOutput(word.Id, word.Name, word.Slug, word.InitialLetter, word.Description);
        }
    }
}
