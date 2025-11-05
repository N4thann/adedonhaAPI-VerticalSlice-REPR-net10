using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infraestructure.Mediator
{
    public class InMemoryMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryMediator(IServiceProvider serviceProvider)  =>  _serviceProvider = serviceProvider;

        public async Task SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            using var scope = _serviceProvider.CreateScope();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            object handler = scope.ServiceProvider.GetRequiredService(handlerType);

            MethodInfo handleMethod = handlerType.GetMethod(
                nameof(ICommandHandler<TCommand>.Handle),
                new[] { typeof(TCommand), typeof(CancellationToken) } 
            );

            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for command {typeof(TCommand).Name}. Ensure it has a CancellationToken parameter.");
            }

            await (Task)handleMethod.Invoke(handler, new object[] { command, cancellationToken });
        }

        public async Task<TResult> SendCommand<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>
        {
            using var scope = _serviceProvider.CreateScope();
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            object handler = scope.ServiceProvider.GetRequiredService(handlerType);

            MethodInfo handleMethod = handlerType.GetMethod(
                nameof(ICommandHandler<TCommand, TResult>.Handle),
                new[] { typeof(TCommand), typeof(CancellationToken) } 
            );

            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for command {typeof(TCommand).Name} with result {typeof(TResult).Name}. Ensure it has a CancellationToken parameter.");
            }

            return await (Task<TResult>)handleMethod.Invoke(handler, new object[] { command, cancellationToken });
        }

        public async Task<TResult> SendQuery<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult> 
        {
            using var scope = _serviceProvider.CreateScope();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            object handler = scope.ServiceProvider.GetRequiredService(handlerType);

            MethodInfo handleMethod = handlerType.GetMethod(
                nameof(IQueryHandler<TQuery, TResult>.Handle),
                new[] { typeof(TQuery), typeof(CancellationToken) }
            );

            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for query {typeof(TQuery).Name} with result {typeof(TResult).Name}. Ensure it has a CancellationToken parameter.");
            }

            return await (Task<TResult>)handleMethod.Invoke(handler, new object[] { query, cancellationToken });
        }
    }
}
