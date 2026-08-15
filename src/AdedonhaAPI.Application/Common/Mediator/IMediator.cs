namespace AdedonhaAPI.Application.Common.Mediator
{
    public interface IMediator
    {
        Task<TOutput> SendAsync<TOutput>(IInput<TOutput> input, CancellationToken cancellationToken = default);
    }
}
