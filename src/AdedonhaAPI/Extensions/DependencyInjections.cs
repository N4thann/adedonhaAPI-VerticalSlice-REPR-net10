using AdedonhaAPI.Application.Common.Context;
using AdedonhaAPI.Application.Common.Options;
using AdedonhaAPI.Common.Context;
using AdedonhaAPI.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace AdedonhaAPI.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            const string corsPolicyName = "AllowMyClient";
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicyName, policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("X-Operation-Id");
                });
            });

            var jwtSettingsSection = configuration.GetSection(JwtOptions.ConfigSectionName);
            services.Configure<JwtOptions>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JwtOptions>();

            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
                throw new ArgumentException("Configurações JWT (SecretKey) não encontradas ou inválidas.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = jwtSettings.ValidAudience,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            var rateLimitOptions = new MyRateLimitOptions();
            configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(rateLimitOptions);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(policyName: "fixedwindow", opt =>
                {
                    opt.PermitLimit = rateLimitOptions.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(rateLimitOptions.Window);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = rateLimitOptions.QueueLimit;
                });

                options.AddSlidingWindowLimiter(policyName: "sliding", opt =>
                {
                    opt.PermitLimit = rateLimitOptions.PermitLimit;
                    opt.Window = TimeSpan.FromSeconds(rateLimitOptions.Window);
                    opt.SegmentsPerWindow = rateLimitOptions.SegmentsPerWindow;
                    opt.QueueLimit = rateLimitOptions.QueueLimit;
                });
            });

            services.AddScoped<RequestContext>();
            services.AddScoped<IRequestContext>(sp => sp.GetRequiredService<RequestContext>());

            return services;
        }
    }
}
