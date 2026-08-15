using AdedonhaAPI.Application.Common.Options;
using AdedonhaAPI.Application.Common.Storage;
using Microsoft.Extensions.Options;

namespace AdedonhaAPI.Infrastructure.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;

        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
            Directory.CreateDirectory(_options.UploadsPath);
        }

        public async Task<string> SaveAsync(FileUploadDto file, CancellationToken cancellationToken)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(_options.UploadsPath, fileName);
            await using (var destination = File.Create(fullPath))
                await file.Content.CopyToAsync(destination, cancellationToken);
            return $"{_options.PublicBasePath}/{fileName}";
        }

        public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken)
        {
            var fileName = Path.GetFileName(fileUrl);
            var fullPath = Path.Combine(_options.UploadsPath, fileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
            return Task.CompletedTask;
        }
    }
}
