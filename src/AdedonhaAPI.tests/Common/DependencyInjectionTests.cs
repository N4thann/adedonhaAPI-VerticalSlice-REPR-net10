using AdedonhaAPI.Application;
using AdedonhaAPI.Application.Common.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace AdedonhaAPI.tests.Common
{
    public class DependencyInjectionTests
    {
        [Fact(DisplayName = "SUCESSO - Deve registrar o IMediator ao chamar AddApplicationServices")]
        public void AddApplicationServices_WhenCalled_ShouldRegisterMediator()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var provider = services.BuildServiceProvider();

            // Assert
            provider.GetService<IMediator>().ShouldNotBeNull();
        }
    }
}
