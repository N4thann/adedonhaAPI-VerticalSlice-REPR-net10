namespace AdedonhaAPI.Shared.Options
{
    public class JwtOptions
    {
        public const string ConfigSectionName = "JWT";

        public string ValidAudience { get; init; }
        public string ValidIssuer { get; init; }
        public string SecretKey { get; init; }
        public int TokenValidityInMinutes { get; init; }
        public int RefreshTokenValidInMinutes { get; init; }
    }
}
