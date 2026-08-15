using System.Reflection;

namespace AdedonhaAPI.Application.Common.Mediator
{
    public class InMemoryMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryMediator(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<TOutput> SendAsync<TOutput>(IInput<TOutput> input, CancellationToken cancellationToken = default)
        {
            var inputType = input.GetType();
            var useCaseType = typeof(IUseCase<,>).MakeGenericType(inputType, typeof(TOutput));

            var useCase = _serviceProvider.GetService(useCaseType);

            if (useCase == null)
                throw new InvalidOperationException($"Nenhum UseCase registrado para manipular a entrada: {inputType.Name}");

            MethodInfo? executeMethod = useCaseType.GetMethod("ExecuteAsync");

            if (executeMethod == null)
                throw new InvalidOperationException($"Método ExecuteAsync não encontrado no UseCase para {inputType.Name}.");

            return await (Task<TOutput>)executeMethod.Invoke(useCase, new object[] { input, cancellationToken })!;
        }
    }
}
