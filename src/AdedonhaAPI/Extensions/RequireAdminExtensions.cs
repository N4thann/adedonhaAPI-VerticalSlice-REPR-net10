using AdedonhaAPI.Application.Common.Identity;

namespace AdedonhaAPI.Extensions
{
    public static class RequireAdminExtensions
    {
        public static RouteHandlerBuilder RequireAdmin(this RouteHandlerBuilder builder) =>
            builder.RequireAuthorization(policy => policy.RequireRole(IdentityRoles.Admin));
    }
}
