namespace AdedonhaAPI.Application.Features.Admin.Words.BulkUploadWordsCsv
{
    public record BulkUploadRowError(int Line, string Reason);

    public record BulkUploadWordsCsvOutput(
        int TotalRows,
        int CategoriesCreated,
        int WordsCreated,
        int AssociationsCreated,
        int RowsSkipped,
        IReadOnlyList<BulkUploadRowError> Errors);
}
