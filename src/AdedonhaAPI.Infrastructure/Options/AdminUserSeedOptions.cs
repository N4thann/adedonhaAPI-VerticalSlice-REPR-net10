namespace AdedonhaAPI.Infrastructure.Options
{
    public class AdminUserSeedOptions
    {
        public const string ConfigSectionName = "AdminUserSeed";

        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
