using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Words.EditWord
{
    /// <summary>
    /// Validacao de borda para a edicao de palavra.
    /// </summary>
    public class EditWordValidator : AbstractValidator<EditWordInput>
    {
        public EditWordValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O id da palavra é obrigatório.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da palavra é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da palavra deve ter no máximo 100 caracteres.");
        }
    }
}
