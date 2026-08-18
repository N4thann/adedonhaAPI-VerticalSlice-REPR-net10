using FluentValidation;

namespace AdedonhaAPI.Application.Features.Auth.Login
{
    /// <summary>
    /// Validacao de borda para o login.
    /// </summary>
    public class LoginValidator : AbstractValidator<LoginInput>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail em formato inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
