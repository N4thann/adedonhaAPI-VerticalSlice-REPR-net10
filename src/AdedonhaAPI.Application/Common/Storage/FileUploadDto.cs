namespace AdedonhaAPI.Application.Common.Storage
{
    public record FileUploadDto(Stream Content, string FileName, string ContentType, long Length);
}
