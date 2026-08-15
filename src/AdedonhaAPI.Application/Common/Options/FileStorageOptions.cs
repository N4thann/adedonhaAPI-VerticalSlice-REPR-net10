namespace AdedonhaAPI.Application.Common.Options
{
    public class FileStorageOptions
    {
        public const string ConfigSectionName = "FileStorage";

        public string UploadsPath { get; init; } = "wwwroot/uploads";
        public string PublicBasePath { get; init; } = "/uploads";
        public long MaxFileSizeBytes { get; init; } = 5_000_000;
        public string[] AllowedImageContentTypes { get; init; } = ["image/jpeg", "image/png", "image/webp"];
    }
}
