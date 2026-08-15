using AdedonhaAPI.Application.Common.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace AdedonhaAPI.tests.Common.Mediator
{
    public class InMemoryMediatorTests
    {
        private interface IScopedMarker { Guid InstanceId { get; } }
        private class ScopedMarker : IScopedMarker { public Guid InstanceId { get; } = Guid.NewGuid(); }
        private record TestInput : IInput<TestOutput>;
        private record TestOutput(Guid MarkerInstanceId);

        private class TestUseCase : IUseCase<TestInput, TestOutput>
        {
            private readonly IScopedMarker _marker;
            public TestUseCase(IScopedMarker marker) => _marker = marker;
            public Task<TestOutput> ExecuteAsync(TestInput input, CancellationToken cancellationToken) =>
                Task.FromResult(new TestOutput(_marker.InstanceId));
        }

        [Fact(DisplayName = "SUCESSO - Deve resolver e invocar o UseCase correto a partir do tipo do input")]
        public async Task SendAsync_WhenInputHasRegisteredUseCase_ShouldInvokeIt()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<IScopedMarker, ScopedMarker>();
            services.AddScoped<IUseCase<TestInput, TestOutput>, TestUseCase>();
            services.AddScoped<IMediator, InMemoryMediator>();
            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var expectedMarker = scope.ServiceProvider.GetRequiredService<IScopedMarker>();

            // Act
            var result = await mediator.SendAsync(new TestInput());

            // Assert
            result.MarkerInstanceId.ShouldBe(expectedMarker.InstanceId);
        }

        [Fact(DisplayName = "SUCESSO - Deve reaproveitar o mesmo escopo Scoped da requisicao em chamadas sucessivas")]
        public async Task SendAsync_WhenCalledTwiceInSameScope_ShouldReuseScopedInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<IScopedMarker, ScopedMarker>();
            services.AddScoped<IUseCase<TestInput, TestOutput>, TestUseCase>();
            services.AddScoped<IMediator, InMemoryMediator>();
            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var expectedMarker = scope.ServiceProvider.GetRequiredService<IScopedMarker>();

            // Act
            var firstResult = await mediator.SendAsync(new TestInput());
            var secondResult = await mediator.SendAsync(new TestInput());

            // Assert
            firstResult.MarkerInstanceId.ShouldBe(expectedMarker.InstanceId);
            secondResult.MarkerInstanceId.ShouldBe(expectedMarker.InstanceId);
        }

        [Fact(DisplayName = "ERRO - Deve lancar InvalidOperationException quando nao houver UseCase registrado para o input")]
        public async Task SendAsync_WhenNoUseCaseRegistered_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<IMediator, InMemoryMediator>();
            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // Act
            var act = async () => await mediator.SendAsync(new TestInput());

            // Assert
            await Should.ThrowAsync<InvalidOperationException>(act);
        }
    }
}
