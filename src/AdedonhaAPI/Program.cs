using AdedonhaAPI.Extensions;
using AdedonhaAPI.Shared.Data;
using Asp.Versioning;
using Carter;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCarter();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdedonhaAPI",
        Version = "v1",
        Description = "API para catálogo de palavras e categorias para o jogo adedonha. " +
        "Possuí modulo admin para cadastro de palavras e categorias.",
        Contact = new OpenApiContact
        {
            Name = "Nathan Farias",
            Email = "francisco.nathan2@outlook.com",
            Url = new Uri("https://www.linkedin.com/in/nathan-farias-5bb97a24"),
        },
        License = new OpenApiLicense
        {
            Name = "Exemplo",
            Url = new Uri("https://github.com/N4thann"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT ",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.EnableAnnotations();
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "AdedonhaAPI"));
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.MapCarter();

app.UseAuthorization();

app.Run();
