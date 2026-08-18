namespace AdedonhaAPI.Application.Common.Identity
{
    public interface IIdentityService
    {
        Task<IdentityUserDto?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
    }
}
