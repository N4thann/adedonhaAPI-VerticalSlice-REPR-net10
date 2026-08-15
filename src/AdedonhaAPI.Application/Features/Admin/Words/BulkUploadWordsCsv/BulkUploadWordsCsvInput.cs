using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv
{
    public record BulkUploadWordsCsvInput(IReadOnlyList<string> Lines) : IInput<ErrorOr<BulkUploadWordsCsvOutput>>;
}
