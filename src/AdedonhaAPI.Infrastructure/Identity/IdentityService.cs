using AdedonhaAPI.Application.Common.Identity;
using Microsoft.AspNetCore.Identity;

namespace AdedonhaAPI.Infrastructure.Identity
{
    /// <summary>
    /// Implementacao de IIdentityService usando o UserManager do ASP.NET Core Identity (MongoDB).
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUserDto?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new IdentityUserDto(user.Id.ToString(), user.Name, user.Email!, roles.ToList());
        }
    }
}
