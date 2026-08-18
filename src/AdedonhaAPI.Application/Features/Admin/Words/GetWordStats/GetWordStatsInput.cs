using AdedonhaAPI.Application.Common.Mediator;
using ErrorOr;

namespace AdedonhaAPI.Application.Features.Admin.Words.GetWordStats
{
    public record GetWordStatsInput() : IInput<ErrorOr<GetWordStatsOutput>>;
}
