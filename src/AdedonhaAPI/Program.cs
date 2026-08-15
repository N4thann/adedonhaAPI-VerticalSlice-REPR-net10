using Asp.Versioning;
using AdedonhaAPI.Application;
using AdedonhaAPI.Extensions;
using AdedonhaAPI.Infrastructure;
using Carter;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.GoogleCloudLogging;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();

    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{OperationId}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                path: "logs/adedonha-log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{OperationId}] {Message:lj}{NewLine}{Exception}");
    }
    else
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions
            {
                ProjectId = context.Configuration["GCP:ProjectId"]
            });
    }
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCarter();

builder.Services.AddOpenApiDocument(settings =>
{
    settings.PostProcess = document =>
    {
        document.Info.Title = "Adedonha API";
        document.Info.Version = "v1";
        document.Info.Description = "API para gerenciar e consultar um repositório de palavras separado por categorias.";

        document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "Nathan Farias",
            Email = "francisco.nathan2@outlook.com",
            Url = "https://www.linkedin.com/in/nathan-farias-5bb97a24"
        };

        document.Info.License = new NSwag.OpenApiLicense
        {
            Name = "Exemplo",
            Url = "https://github.com/N4thann"
        };
    };

    settings.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Insira o token JWT: Bearer {seu_token}",
    });

    settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
});

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader(),
        new UrlSegmentApiVersionReader());
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

app.UseOperationId();
app.ConfigureExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(settings => settings.Path = "/openapi/{documentName}/openapi.json");
    app.UseSwaggerUi(settings =>
    {
        settings.DocumentPath = "/openapi/{documentName}/openapi.json";
        settings.DocumentTitle = "Adedonha API - Docs";
    });
}

app.UseStaticFiles();
app.UseCors("AllowMyClient");
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestContext();
app.UseRateLimiter();
app.MapCarter();

app.Run();
