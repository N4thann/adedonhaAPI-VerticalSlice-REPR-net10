using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategories
{
    /// <summary>
    /// Validacao de borda para os parametros de paginacao/busca de categorias.
    /// </summary>
    public class GetCategoriesValidator : AbstractValidator<GetCategoriesInput>
    {
        public GetCategoriesValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}
