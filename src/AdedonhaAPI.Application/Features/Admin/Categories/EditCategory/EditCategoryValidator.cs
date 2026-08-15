using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Categories.EditCategory
{
    /// <summary>
    /// Validacao de borda para a edicao de categoria.
    /// </summary>
    public class EditCategoryValidator : AbstractValidator<EditCategoryInput>
    {
        public EditCategoryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O id da categoria é obrigatório.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");
        }
    }
}
