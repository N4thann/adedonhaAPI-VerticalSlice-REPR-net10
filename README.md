# AdedonhaAPI

Repositório de palavras para o jogo Adedonha (Stop!) — um catálogo de palavras organizado por
categoria e letra inicial, com uma API .NET 10 e um frontend React que consome essa API.

Monorepo: backend em `src/` (.NET 10) e frontend em `AdedonhaWeb/` (Vite + React + TypeScript).

## O produto

A aplicação tem dois módulos:

**Catálogo (público)**
- Mural com todas as categorias ativas (ex.: Animais, Marcas, Profissão).
- Ao entrar em uma categoria e escolher uma letra, mostra as palavras daquela combinação
  categoria/letra em ordem embaralhada (determinística por sessão, via seed), paginadas com um
  botão "Mostrar mais".
- Detalhe de cada palavra (descrição) em um dialog.
- Estatística agregada: total de palavras cadastradas e quantas palavras existem por categoria,
  em um gráfico donut.
- Página "Sobre o site".

**Admin (protegido por JWT)**
- CRUD completo de Categorias e Palavras.
- Associação/desassociação de palavras a categorias.
- Import em massa de palavras via upload de CSV.
- Filtro por categoria e busca por nome na listagem de palavras.
- Dashboard com o mesmo gráfico de palavras por categoria, mais tema de cores configurável.
- Edição do conteúdo da página "Sobre o site" (texto + foto).

## Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 10, ASP.NET Core (Minimal APIs), C# |
| Roteamento de endpoints | [Carter](https://github.com/CarterCommunity/Carter) |
| Persistência | MongoDB (`MongoDB.Driver`), sem migrations — índices criados em runtime |
| Autenticação | JWT Bearer + ASP.NET Core Identity (`AspNetCore.Identity.MongoDbCore`) |
| Validação | FluentValidation |
| Result Pattern | [ErrorOr](https://github.com/amantinband/error-or) — sem exceptions para erros de negócio esperados |
| Versionamento de API | Asp.Versioning.Http |
| Rate limiting | `Microsoft.AspNetCore.RateLimiting` nativo (fixed window + sliding window) |
| Logging | Serilog — Console/arquivo em Dev, Google Cloud Logging em produção |
| Documentação de API | NSwag/OpenAPI, com esquema de segurança Bearer |
| Testes de backend | xUnit, NSubstitute, Shouldly, Bogus |
| Frontend | React 19, TypeScript, Vite |
| UI | MUI (Material UI) + `@mui/x-charts` |
| Estado | Zustand (estado de domínio) + React Context (tema, sessão) |
| HTTP client | Axios |
| Roteamento | React Router |
| Testes de frontend | Vitest + Testing Library |

## Arquitetura

### Backend: Vertical Slice Architecture + padrão REPR

Em vez de camadas técnicas horizontais (Controllers → Services → Repositories) compartilhadas por
toda a aplicação, o código é organizado **por feature**. Cada caso de uso é uma "fatia vertical"
autocontida sob `Features/{Admin|Catalog}/{Área}/{CasoDeUso}/`, com tudo que aquele caso de uso
precisa: request/input, validação, regra de negócio e response/output. Isso mantém alta coesão
(o que muda junto fica junto) e baixo acoplamento (uma feature não sabe da existência da outra).

Dentro de cada fatia, o fluxo segue o padrão **REPR** (Request → Endpoint → Response):

- **Endpoint**: uma classe `ICarterModule` que só mapeia rota, verbo HTTP e política de
  autorização, e delega para o mediator. Não tem lógica de negócio.
- **Request/Input**: um `record` que modela a entrada (equivalente a um Command/Query).
- **UseCase**: implementa `IUseCase<TInput, TOutput>` — é aqui que a regra de negócio mora,
  validada por um `IValidator<TInput>` (FluentValidation) e retornando `ErrorOr<TOutput>` em vez
  de lançar exceptions para erros esperados (validação, not found, forbidden, conflito).
- **Response/Output**: um `record` que modela a saída.

O código do projeto ainda mantém as camadas de solução tradicionais do .NET
(`AdedonhaAPI.Domain` → `AdedonhaAPI.Application` → `AdedonhaAPI.Infrastructure` →
`AdedonhaAPI`/apresentação), mas a organização *dentro* da camada `Application` é por feature, não
por tipo técnico.

### Mediator manual (sem MediatR)

Em vez de uma biblioteca de terceiros, o projeto define sua própria abstração mínima de mediator:
`IUseCase<TInput, TOutput>` como contrato de caso de uso e `IMediator`/`InMemoryMediator` como
despachante, que resolve o handler certo via reflection e injeção de dependência. Um novo caso de
uso só precisa implementar `IUseCase<>` — não há registro manual no DI, os handlers são
descobertos automaticamente escaneando o assembly.

### Módulos Admin vs. Catalog: duplicação deliberada

Quando uma mesma informação é exposta tanto para o Admin (autenticado) quanto para o público
(Catalog, anônimo) — por exemplo, contagem de palavras por categoria — cada módulo tem seu **próprio**
use case, endpoint e tipo de saída, mesmo que a lógica seja parecida. A alternativa (um único use
case compartilhado) acoplaria a política de autorização e a evolução de um módulo à do outro; a
duplicação aqui é intencional, não descuido.

### Outros padrões usados

- **Result Pattern (`ErrorOr`)**: toda regra de negócio que pode falhar de forma esperada
  (validação, recurso não encontrado, conflito, acesso negado) retorna um `ErrorOr<T>` em vez de
  lançar exception. Exceptions ficam reservadas para falhas realmente excepcionais.
- **Options Pattern**: configuração fortemente tipada (`JwtOptions`, `MongoDbConfigOptions`,
  `MyRateLimitOptions`) validada e injetada via `IOptions<T>`, nunca lida diretamente de
  `IConfiguration` dentro da lógica de negócio.
- **Slugs determinísticos**: categorias e palavras têm um `Slug` gerado a partir do nome
  (normalizado, sem acento, sem caracteres especiais) — é o identificador usado nas rotas públicas
  (`/catalog/categories/{slug}`), com índice único no Mongo para evitar colisão.
- **Índices criados em runtime**: sem sistema de migrations — um `IHostedService`
  (`MongoDbIndexService`) garante os índices do MongoDB na subida da aplicação.
- **Rate limiting nativo**: políticas de fixed window e sliding window configuradas via Options
  Pattern, usando o limitador embutido do ASP.NET Core.
- **Logging estruturado com Serilog**: enrichment consistente entre ambientes — Console/arquivo
  em desenvolvimento, Google Cloud Logging em produção — com um Operation ID por requisição.

### Frontend: Atomic Design + slices por domínio

- **Componentes** organizados por Atomic Design: `components/atoms|molecules|organisms|templates`.
- **Cada domínio** (palavra, categoria, sobre-o-site, dashboard) repete a mesma fatia vertical:
  `types/{domínio}.types.ts` → `services/{domínio}Service.ts` (chamadas Axios) →
  `store/{domínio}/use{Domínio}Store.ts` (Zustand, dono do estado de loading/erro) → páginas e
  componentes que consomem a store.
- **Estado global e transversal** (tema de cores, sessão JWT do Admin) fica em React Context, não
  em Zustand — Zustand é reservado para estado de domínio por feature.
- **Módulo público vs. Admin**: rotas, serviços e stores do Catálogo público (`catalog*`) são
  isolados dos equivalentes do Admin, espelhando a mesma duplicação deliberada do backend.

## Estrutura de pastas

```
src/
├── AdedonhaAPI/                    # Camada de apresentação: Endpoints (Carter), Program.cs
├── AdedonhaAPI.Application/        # Casos de uso (Features/Admin, Features/Catalog), validators, mediator
├── AdedonhaAPI.Domain/             # Entidades (MongoDB POCOs), interfaces de repositório, SlugGenerator
├── AdedonhaAPI.Infrastructure/     # MongoDbContext, índices, implementações de repositório
└── AdedonhaAPI.tests/              # Testes de UseCases e Validators (xUnit/NSubstitute/Shouldly)

AdedonhaWeb/
└── src/
    ├── components/                 # atoms, molecules, organisms, templates
    ├── contexts/                   # Tema de cores, sessão Admin
    ├── pages/                      # Páginas públicas + pages/admin
    ├── routes/                     # Definição de rotas e proteção de rotas
    ├── services/                   # Chamadas Axios por domínio
    ├── store/                      # Zustand stores por domínio
    └── types/                      # Tipos TypeScript por domínio
```

## Rodando localmente

Pré-requisitos: .NET 10 SDK, Node.js, uma instância MongoDB acessível.

### Backend

```bash
cd src
dotnet build AdedonhaAPI.sln
```

A aplicação exige configuração que **não** vem em `appsettings.json` (lançam erro se ausentes) —
defina via `dotnet user-secrets` (o projeto já tem `UserSecretsId` configurado) ou variáveis de
ambiente:

- `JWT`: `ValidAudience`, `ValidIssuer`, `SecretKey`, `TokenValidityInMinutes`, `RefreshTokenValidInMinutes`
- `MongoDbConfig`: `Name`, `Host`, `Port`

`dotnet run` é sempre manual — suba a API pela IDE ou CLI conforme sua preferência. URLs locais
padrão: `http://localhost:5055` (perfil http) / `https://localhost:7295` (perfil https). Swagger UI
em `/openapi/{documentName}/openapi.json`, disponível apenas em Development.

### Frontend

```bash
cd AdedonhaWeb
npm install
cp .env.example .env   # defina VITE_API_BASE_URL apontando para a API
npm run dev
```

### Testes

```bash
# Backend
dotnet test src/AdedonhaAPI.sln

# Frontend
cd AdedonhaWeb
npm test
```
