using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Categories.GetCategoryWordCounts
{
    public record GetCategoryWordCountsInput() : IInput<ErrorOr<GetCategoryWordCountsOutput>>;
}
