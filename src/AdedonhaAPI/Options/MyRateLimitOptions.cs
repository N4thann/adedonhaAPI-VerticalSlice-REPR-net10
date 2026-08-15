namespace AdedonhaAPI.Options
{
    public class MyRateLimitOptions
    {
        public const string MyRateLimit = "MyRateLimit";
        public int PermitLimit { get; set; } = 10;
        public int Window { get; set; } = 10;
        public int QueueLimit { get; set; } = 2;
        public int SegmentsPerWindow { get; set; } = 4;
    }
}
