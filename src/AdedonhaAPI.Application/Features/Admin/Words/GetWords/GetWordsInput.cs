using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWords
{
    public record GetWordsInput(int Page, int PageSize, string? Search) : IInput<ErrorOr<GetWordsOutput>>;
}
