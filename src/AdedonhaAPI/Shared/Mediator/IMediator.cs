namespace Application.Interfaces
{
    public interface IMediator
    {
        Task SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;

        Task<TResult> SendCommand<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>;

        Task<TResult> SendQuery<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>;//Em vez de chamar de Send usamos o Query para não dar conflito
    }
}
