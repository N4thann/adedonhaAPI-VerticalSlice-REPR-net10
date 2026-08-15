using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Catalog.GetCatalogCategoryWords
{
    public record GetCatalogCategoryWordsInput(string CategorySlug, int Page, int PageSize, char? Letter, string? Search)
        : IInput<ErrorOr<GetCatalogCategoryWordsOutput>>;
}
