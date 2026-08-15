namespace AdedonhaAPI.Domain.Common
{
    public record PagedResult<T>(IReadOnlyList<T> Items, long TotalCount, int Page, int PageSize);
}
