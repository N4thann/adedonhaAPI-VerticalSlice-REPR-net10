namespace AdedonhaAPI.Application.Common.Options
{
    public class JwtOptions
    {
        public const string ConfigSectionName = "JWT";

        public string ValidAudience { get; init; } = string.Empty;
        public string ValidIssuer { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public int TokenValidityInMinutes { get; init; }
        public int RefreshTokenValidInMinutes { get; init; }
    }
}
