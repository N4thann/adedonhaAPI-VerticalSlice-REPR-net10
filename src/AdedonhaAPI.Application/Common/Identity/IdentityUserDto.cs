namespace AdedonhaAPI.Application.Common.Identity
{
    public record IdentityUserDto(string Id, string Name, string Email, IReadOnlyList<string> Roles);
}
