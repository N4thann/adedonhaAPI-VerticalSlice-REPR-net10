using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Auth.Login
{
    public record LoginInput(string Email, string Password) : IInput<ErrorOr<LoginOutput>>;
}
