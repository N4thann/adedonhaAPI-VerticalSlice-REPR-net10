# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

AdedonhaAPI is a .NET 10 REST API that serves as a word repository for the game Adedonha (Stop!). It exposes two modules: a public **Catalog** module (browse categories/words) and a JWT-protected **Admin** module (CRUD for categories/words, bulk CSV import).

The actual project lives at `src/AdedonhaAPI/` (solution: `src/AdedonhaAPI.sln`). There is currently no test project in the solution.

**Status: early scaffold.** Several files are stubs or contain leftover code copied from another project (see "Known inconsistencies" below) — don't assume existing code is a correct pattern to copy from until it's been reconciled.

## Commands

Run from `src/AdedonhaAPI/` (or pass `src/AdedonhaAPI.sln` explicitly):

```
dotnet build src/AdedonhaAPI.sln
dotnet test src/AdedonhaAPI.sln          # no test project exists yet
```

Per the user's global rules, `dotnet run` is always run manually by the user — never invoke it. Build/test may be run as a final verification step after implementing something.

Local URLs (from `Properties/launchSettings.json`): `http://localhost:5055` (http profile), `https://localhost:7295` / `http://localhost:5055` (https profile). Swagger/OpenAPI UI is served at `/openapi/{documentName}/openapi.json` (doc) and via `UseSwaggerUi` in Development only.

## Required configuration (not in appsettings.json)

`appsettings.json` only has `Logging`/`AllowedHosts`. Two config sections are read at startup and **throw on missing/empty values**, so they must be supplied via user secrets or environment variables before the app will boot:

- `JWT` → bound to `Shared/Options/JwtOptions.cs` (`ValidAudience`, `ValidIssuer`, `SecretKey`, `TokenValidityInMinutes`, `RefreshTokenValidInMinutes`).
- `MongoDbConfig` → bound to `Shared/Options/MongoDbConfigOptions.cs` (`Name`, `Host`, `Port`; `ConnectionString` is derived as `mongodb://{Host}:{Port}`).
- `MyRateLimit` (optional, has defaults) → `Shared/Options/MyRateLimitOptions.cs`.

The project's `UserSecretsId` is already set in the `.csproj`, so `dotnet user-secrets` works out of the box.

## Architecture

**Vertical Slice Architecture with the REPR pattern** (Request-Endpoint-Response) — no MVC controllers, no layered Service/Repository split. Each feature lives entirely under `Features/{Admin|Catalog}/{Area}/{UseCase}/` and owns its own Command/Query, Endpoint, and Handler.

- **Carter** registers HTTP endpoints. Each feature endpoint is a `CarterModule` subclass that maps its own route(s) in `AddRoutes`; `builder.Services.AddCarter()` + `app.MapCarter()` in `Program.cs` wire all modules up automatically — no manual registration per feature.
- **Manual mediator** (no MediatR): `ICommand`/`ICommand<TResult>`, `ICommandHandler<TCommand>`/`ICommandHandler<TCommand,TResult>`, `IQuery<TResult>`, `IQueryHandler<TQuery,TResult>` define the contracts; `InMemoryMediator` implements `IMediator` and dispatches via reflection (`SendCommand`, `SendQuery`). Handlers are auto-discovered and registered as `Scoped` by scanning the entry assembly in `Extensions/DependencyInjections.cs` (`RegisterCommandHandlers`, `RegisterCommandHandlersWithResult`, `RegisterQueryHandlersWithResult`) — a new handler class just needs to implement the right interface, no manual DI registration.
- **MongoDB** is the only persistence store, accessed through `Shared/Data/Context.cs` (exposes `Words` and `Categories` collections, `Scoped` in DI). Identity (`ApplicationUser`, `ApplicationRole`, both keyed by `Guid`) is also persisted in Mongo via `AspNetCore.Identity.MongoDbCore`.
- **Indexes are created at startup**, not via migrations: `AdedonhaAPI.Infrastructure/Database/MongoDbIndexService.cs` is an `IHostedService` that creates unique indexes on `Category.Slug` and `Word.Slug`; a compound index on `Word.Categories.CategoryId + Word.InitialLetter + Word.Name` (the "10 random words per letter/category" catalog view and the public mural's per-category random sample); and compound indexes on `Word.IsActive + Word.Name` / `Category.IsActive + Category.Name` for the paginated Admin listings. New query patterns should generally get a matching index added here rather than relying on collection scans.
- **Domain models** (`Shared/Domain/Category.cs`, `Shared/Domain/Word.cs`) are Mongo POCOs with `[BsonId]`/`[BsonElement]` attributes — `Category.Id` is a string, `Word.Id` is a `Guid` stored as string. `Word` embeds a denormalized `List<CategoryInfo>` (CategoryId/Slug/Name) rather than referencing categories by id alone, which is what the compound indexes above are built against.
- **Auth**: JWT bearer (`AddJwtBearer`), with `AdminOnly`/`SuperAdminOnly`/`UserOnly` role policies plus one claims-based `ExclusivePolicyOnly` policy, all defined in `Extensions/DependencyInjections.cs`. NSwag is configured with a `Bearer` security scheme so Swagger UI can send tokens directly.
- **Rate limiting**: ASP.NET Core native limiter, configured via `MyRateLimitOptions` — a `fixedwindow` policy bound to those options and a hardcoded `sliding` policy, both registered but not yet applied to any endpoint via `.RequireRateLimiting(...)`.
- **API versioning**: `Asp.Versioning.Http`, reading version from query string or URL segment, default `1.0`.
- Global exception handling middleware (`Extensions/ApiExceptionMiddleware.cs` + `ErrorDetailsOutput`) is only wired up in `app.Environment.IsDevelopment()` in `Program.cs`.

## Data seed

`adedonha_palavras.csv` at the project root (`Id,Categoria,Letra,Palavra`) is the seed source for words/categories — not yet wired into a seeding mechanism.

## Known inconsistencies to reconcile during the refactor

- The mediator abstractions under `Shared/Mediator/*.cs` are declared in namespaces `Application.Interfaces` (contracts) and `Infraestructure.Mediator` (`InMemoryMediator`) — mismatched against their actual folder (`Shared/Mediator`) and the rest of the codebase's `AdedonhaAPI.*` namespace convention. `DependencyInjections.cs` already imports `Application.Interfaces` / `Infraestructure.Mediator` to compile against them.
- Several feature files are empty stubs or placeholders: `Features/Admin/Roles/CreateRole/CreateRoleEndpoint.cs` is an empty class; `Features/Catalog/Words/GetPaginationWords/GetPaginationWordsEndpoint.cs` returns a hardcoded `"Hello Carter"` string instead of querying Mongo.
- `AdedonhaAPI.csproj` still declares empty `<Folder Include="..."/>` entries for features that don't exist yet as code (`Categories`, `Users/Login`, `Words/CreateWord|DeleteWord|GetWord|EditWord`), reflecting the intended-but-unbuilt feature set.
