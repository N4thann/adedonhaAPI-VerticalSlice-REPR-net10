using AdedonhaAPI.Application.Common.Mediator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AdedonhaAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var applicationAssembly = typeof(DependencyInjection).Assembly;

            services.AddScoped<IMediator, InMemoryMediator>();

            var useCaseTypes = applicationAssembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUseCase<,>))
                            && !t.IsAbstract && !t.IsInterface);

            foreach (var type in useCaseTypes)
            {
                var interfaceType = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IUseCase<,>));
                services.AddScoped(interfaceType, type);
            }

            services.AddValidatorsFromAssembly(applicationAssembly);

            return services;
        }
    }
}
