using AdedonhaAPI.Extensions;
using AdedonhaAPI.Shared.Data;
using Asp.Versioning;
using Carter;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCarter();

builder.Services.AddOpenApiDocument(settings =>
{
    settings.PostProcess = document =>
    {
        document.Info.Title = "Adedonha API";
        document.Info.Version = "v1"; // Será sobrescrito pelo versionamento
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

    settings.OperationProcessors.Add(
        new OperationSecurityScopeProcessor("Bearer"));

});

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader()
    );
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddWebApiServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddHostedService<MongoDbIndexService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureExceptionHandler();
    // 1. O Gerador (onde o arquivo .json é criado)
    // Precisamos dizer a ele para usar o MESMO caminho que a UI espera.
    app.UseOpenApi(settings =>
    {
        settings.Path = "/openapi/{documentName}/openapi.json";
    });

    // 2. A UI (onde o usuário vê)
    // (O seu já estava quase certo, apontando para o caminho correto)
    app.UseSwaggerUi(settings =>
    {
        // Este caminho DEVE ser o mesmo do 'settings.Path' acima
        settings.DocumentPath = "/openapi/{documentName}/openapi.json";
        settings.DocumentTitle = "Adedonha API - Docs";
    });
}

app.UseHttpsRedirection();

app.MapCarter();

app.UseAuthorization();

app.Run();
