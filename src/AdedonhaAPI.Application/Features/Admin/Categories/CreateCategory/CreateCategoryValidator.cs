using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Categories.CreateCategory
{
    /// <summary>
    /// Validacao de borda para a criacao de categoria.
    /// </summary>
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryInput>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");
        }
    }
}
