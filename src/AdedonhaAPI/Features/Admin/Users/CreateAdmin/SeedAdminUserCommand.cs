using Application.Interfaces;

namespace AdedonhaAPI.Features.Admin.Users.CreateAdmin
{
    public record SeedAdminUserCommand() : ICommand<string>;
}
