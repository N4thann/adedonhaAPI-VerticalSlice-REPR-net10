using FluentValidation;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords
{
    /// <summary>
    /// Validacao de borda para os parametros de paginacao/busca/letra de palavras de uma categoria.
    /// </summary>
    public class GetCatalogCategoryWordsValidator : AbstractValidator<GetCatalogCategoryWordsInput>
    {
        public GetCatalogCategoryWordsValidator()
        {
            RuleFor(x => x.CategorySlug).NotEmpty().WithMessage("O slug da categoria é obrigatório.");
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
            RuleFor(x => x.Letter)
                .Must(letter => !letter.HasValue || char.IsLetter(letter.Value))
                .WithMessage("A letra informada deve ser uma letra válida (A-Z).");
        }
    }
}
