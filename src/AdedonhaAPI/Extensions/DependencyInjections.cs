using AdedonhaAPI.Shared.Data;
using AdedonhaAPI.Shared.Identity;
using AdedonhaAPI.Shared.Options;
using Application.Interfaces;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Infraestructure.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDbGenericRepository;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace AdedonhaAPI.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services,
                                                         IConfiguration configuration)
        {
            #region CORS
            var nomeDaPoliticaCORS = "AllowMyClient";

            services.AddCors(options =>
            {
                options.AddPolicy(name: nomeDaPoliticaCORS,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            #endregion

            #region AUTENTICAÇÃO JWT
            var secretKey = configuration["JWT:SecretKey"] ??
                            throw new ArgumentException("Invalid secret key!!");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
            #endregion

            #region AUTORIZAÇÃO POLICIES
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                        policy.RequireRole("Admin"));

                options.AddPolicy("SuperAdminOnly", policy =>
                        policy.RequireRole("SuperAdmin"));

                options.AddPolicy("UserOnly", policy =>
                        policy.RequireRole("User"));

                options.AddPolicy("ExclusivePolicyOnly", policy =>
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "Nathan")
                    || context.User.IsInRole("SuperAdmin")));
            });
            #endregion

            #region RATE LIMIT
            var myOptions = new MyRateLimitOptions();
            configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(policyName: "fixedwindow", opt =>
                {
                    opt.PermitLimit = myOptions.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(myOptions.Window);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = myOptions.QueueLimit;
                });

                options.AddSlidingWindowLimiter(policyName: "sliding", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.SegmentsPerWindow = 2;
                    opt.QueueLimit = 5;
                });
            });
            #endregion

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                                 IConfiguration configuration)
        {
            var mongoConfigSection = configuration.GetSection(MongoDbConfigOptions.ConfigSectionName);

            services.Configure<MongoDbConfigOptions>(mongoConfigSection);

            var mongoDbConfig = mongoConfigSection.Get<MongoDbConfigOptions>();

            if (mongoDbConfig == null ||
                string.IsNullOrEmpty(mongoDbConfig.ConnectionString) ||
                string.IsNullOrEmpty(mongoDbConfig.Name))
                throw new InvalidOperationException("Configurações do MongoDB não encontradas ou incompletas.");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbConfig.ConnectionString));

            services.AddScoped(sp =>
                    new Context(sp.GetRequiredService<IMongoClient>(),
                    mongoDbConfig.Name));

            //Identity
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
                    options.SignIn.RequireConfirmedEmail = true;
                },
            };

            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(
                mongoDbIdentityConfig)
                .AddDefaultTokenProviders();

            return services;

        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMediator, InMemoryMediator>();

            var applicationAssembly = typeof(ICommand).Assembly;

            services.RegisterCommandHandlers(applicationAssembly);

            services.RegisterCommandHandlersWithResult(applicationAssembly);

            services.RegisterQueryHandlersWithResult(applicationAssembly);

            return services;
        }

        private static IServiceCollection RegisterCommandHandlers(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) && !t.IsAbstract && !t.IsInterface)))
            {
                var commandType = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)).GetGenericArguments()[0];
                var interfaceType = typeof(ICommandHandler<>).MakeGenericType(commandType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }

        private static IServiceCollection RegisterCommandHandlersWithResult(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) && !t.IsAbstract && !t.IsInterface)))
            {
                var genericArguments = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)).GetGenericArguments();
                var commandType = genericArguments[0];
                var resultType = genericArguments[1];
                var interfaceType = typeof(ICommandHandler<,>).MakeGenericType(commandType, resultType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }

        private static IServiceCollection RegisterQueryHandlersWithResult(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) && !t.IsAbstract && !t.IsInterface)))
            {
                var genericArguments = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)).GetGenericArguments();
                var queryType = genericArguments[0];
                var resultType = genericArguments[1];
                var interfaceType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }
    }
}
