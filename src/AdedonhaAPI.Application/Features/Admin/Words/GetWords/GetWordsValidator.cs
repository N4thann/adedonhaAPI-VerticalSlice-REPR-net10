using FluentValidation;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWords
{
    /// <summary>
    /// Validacao de borda para os parametros de paginacao/busca de palavras.
    /// </summary>
    public class GetWordsValidator : AbstractValidator<GetWordsInput>
    {
        public GetWordsValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}
