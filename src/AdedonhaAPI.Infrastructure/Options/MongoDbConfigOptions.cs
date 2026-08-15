namespace AdedonhaAPI.Infrastructure.Options
{
    public class MongoDbConfigOptions
    {
        public const string ConfigSectionName = "MongoDbConfig";

        public string Name { get; init; } = string.Empty;
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
