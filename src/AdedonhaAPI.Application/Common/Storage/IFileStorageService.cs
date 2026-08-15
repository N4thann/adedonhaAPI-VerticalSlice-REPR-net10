namespace AdedonhaAPI.Application.Common.Storage
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(FileUploadDto file, CancellationToken cancellationToken);
        Task DeleteAsync(string fileUrl, CancellationToken cancellationToken);
    }
}
