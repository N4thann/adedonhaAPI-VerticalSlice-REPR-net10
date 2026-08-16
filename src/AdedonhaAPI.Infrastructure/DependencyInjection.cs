using AdedonhaAPI.Application.Common.Options;
using AdedonhaAPI.Application.Common.Storage;
using AdedonhaAPI.Domain.Interfaces;
using AdedonhaAPI.Infrastructure.Database;
using AdedonhaAPI.Infrastructure.Identity;
using AdedonhaAPI.Infrastructure.Options;
using AdedonhaAPI.Infrastructure.Repositories;
using AdedonhaAPI.Infrastructure.Storage;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AdedonhaAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConfigSection = configuration.GetSection(MongoDbConfigOptions.ConfigSectionName);
            services.Configure<MongoDbConfigOptions>(mongoConfigSection);
            var mongoDbConfig = mongoConfigSection.Get<MongoDbConfigOptions>();

            if (mongoDbConfig == null || string.IsNullOrEmpty(mongoDbConfig.ConnectionString) || string.IsNullOrEmpty(mongoDbConfig.Name))
                throw new InvalidOperationException("Configurações do MongoDB não encontradas ou incompletas.");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbConfig.ConnectionString));
            services.AddScoped(sp => new MongoDbContext(sp.GetRequiredService<IMongoClient>(), mongoDbConfig.Name));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = mongoDbConfig.ConnectionString,
                    DatabaseName = mongoDbConfig.Name,
                },
                IdentityOptionsAction = options =>
                {
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false;
                },
            };

            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
                .AddDefaultTokenProviders();

            services.Configure<AdminUserSeedOptions>(configuration.GetSection(AdminUserSeedOptions.ConfigSectionName));
            services.AddHostedService<UserSeederService>();
            services.AddHostedService<MongoDbIndexService>();
            services.AddHostedService<WordsSeederService>();

            services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.ConfigSectionName));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
