namespace AdedonhaAPI.Application.Common.Mediator
{
    public interface IInput<out TOutput> { }

    public interface IUseCase<in TInput, TOutput> where TInput : IInput<TOutput>
    {
        Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken);
    }

    public interface IUseCase<TOutput>
    {
        Task<TOutput> ExecuteAsync(CancellationToken cancellationToken);
    }
}
