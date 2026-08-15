using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Words.CreateWord
{
    /// <summary>
    /// Validacao de borda para a criacao de palavra.
    /// </summary>
    public class CreateWordValidator : AbstractValidator<CreateWordInput>
    {
        public CreateWordValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da palavra é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da palavra deve ter no máximo 100 caracteres.");
        }
    }
}
