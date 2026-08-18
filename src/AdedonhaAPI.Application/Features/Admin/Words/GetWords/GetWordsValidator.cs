using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWords
{
    /// <summary>
    /// Validacao de borda para os parametros de paginacao/busca/filtro de palavras.
    /// </summary>
    public class GetWordsValidator : AbstractValidator<GetWordsInput>
    {
        public GetWordsValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("O identificador da categoria não pode ser vazio.")
                .When(x => x.CategoryId is not null);
        }
    }
}
