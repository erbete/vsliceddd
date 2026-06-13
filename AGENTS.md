# VSliceDDD

Playground for experimenting with vertical slice architecture, backend-for-frontend, and other modern patterns.
A minimal Library (Shelf → Books) where the domain stays small on purpose, the architecture is the experiment, not
the features.
Built with .NET 10 (or newer), htmx + alpine.js, and PostgreSQL.

## Tech Stack

- **Backend**: ASP.NET Core Minimal APIs (.NET 10 / C# 13 – or newer)
- **Frontend**: htmx + alpine.js, served via ASP.NET Core Minimal APIs with `RazorComponentResult<T>`
- **ORM**: Entity Framework Core 10 (or newer) with Npgsql (PostgreSQL or newer)
- **Logging**: Serilog (console + file sinks)

## Project Structure

```
src/
Domain/            # Domain model (aggregates, value objects, domain events, shared types)
Common/            # Result types, base classes, common value objects
Shelves/           # Shelves domain model
Infrastructure/    # Domain-level infrastructure (if any)
Database/          # Data access layer (separate project)
Configurations/    # EF Core entity type configurations
Interceptors/      # EF Core interceptors (e.g. EntityAuditInterceptor)
Migrations/        # EF Core migrations
WebAPI/            # ASP.NET Core backend (API endpoints + feature slices)
Features/          # Vertical slices (endpoints + handlers per feature)
Infrastructure/    # App-level infrastructure (middleware, exception handling, etc.)
Program.cs         # Application entry point
Client/            # ASP.NET Core SSR UI host (htmx + alpine.js) — queries DB directly, not a BFF
Pages/             # Razor page components
Components/        # Reusable Razor components
Infrastructure/    # Client-level infrastructure
wwwroot/           # Static assets
assets/            # Source assets (processed by Vite)
vite.config.js     # Vite bundler configuration
bun.lock           # Bun package lock
Program.cs         # Client entry point
tests/             # Tests
```

## Build & Run

```sh
# Start PostgreSQL
docker compose up -d

# Run the API
dotnet run --project src/WebAPI

# Run the Client (htmx + alpine.js frontend)
dotnet run --project src/Client

# Build the solution
dotnet build

# Add a migration
dotnet ef migrations add <Name> --project src/WebAPI

# Apply migrations
dotnet ef database update --project src/WebAPI
```

## Architecture

The architecture is **Vertical Slice** as the primary organising principle, with selected DDD building blocks where they
add value. There is no repository pattern — EF Core's `DbContext` is used directly as the data access layer.

### Vertical Slices

Features live in `Features/<Feature>/`. Each slice owns its endpoint, request/response types, and handler logic. Slices
are allowed to reach directly into `AppDbContext` and the domain model — no intermediate repository or service
layer between the handler and EF Core.

A slice typically contains:

- `Endpoints.cs` — exposes `Map<Feature>Endpoints()` (registers the feature's Minimal API routes) and
  `Add<Feature>Feature()` (registers the feature's handlers in DI as `Scoped`)
- One file per operation (e.g. `GetShelves.cs`, `AddBookToShelf.cs`) — static class with a `Handle()` delegate

### DDD Building Blocks

DDD is used selectively within the domain model, not as a full architecture:

- **Aggregate hierarchy**: `Entity` → `AggregateRoot`
- **Factory methods**: entities use static `Create()` — no public constructors
- **Encapsulation**: `private set` / `protected init`; mutable collections exposed only as `IReadOnlyList<T>`
- **Value objects**: e.g. `DateRange` (C# records)
- **Result type**: `Result` / `Result<T>` in `Domain.Common`, carrying a `ResultError`
  (`ErrorCode` + message). For outcomes that can legitimately fail — in-aggregate
  business rules (domain) and failures the aggregate can't evaluate from its own state
  (handlers: not-found, cross-aggregate rules, concurrency, authorization). Mapped to
  HTTP centrally (see Error Handling).
- **Domain events**: `IDomainEvent` interface with a collection on the aggregate root (dispatching not yet wired)

No repository pattern. No application service layer. Handlers in feature slices work directly with `AppDbContext`.


### Business Logic Placement

- **In-aggregate rules** (enforceable from one aggregate's own state, e.g. "shelf holds
  ≤ N books") live in the aggregate — never duplicated or reached-around in a handler.
  The aggregate is the rule's guardian (encapsulation: private collections, `internal`
  mutators).
- **Cross-aggregate rules, orchestration, and decisions needing outside data**
  (uniqueness across aggregates, multi-step workflows, anything requiring the DB, clock,
  current user, or another service) live in the handler — or a domain service the
  handler calls. The domain can't reach these.
- Handlers must never re-implement or bypass an in-aggregate rule (e.g. checking
  `shelf.Books.Count` directly instead of letting `AddBook` decide). That drifts.

Rule of thumb: can one aggregate decide it from its own state? → domain. Does it need
other aggregates, the DB, or the request context? → handler.

### Frontend (Client Project — SSR Web Host)

The Client project is a separate ASP.NET Core Minimal APIs server running on its own port. 
It serves HTML pages rendered via RazorComponentResult<T> — Razor components rendered to static HTML (no SignalR, no persistent connection). 
It is a server-side-rendered (SSR) UI host, not a BFF: it does not proxy or call WebAPI. 
It queries the database directly through its own AppDbContext, sitting at the same tier as WebAPI rather than in front of it (Browser → Client → DB, parallel to Browser → WebAPI → DB). 
A true BFF would source its data by calling a backend API and own no schema; Client owns data access, so the BFF label does not apply. 
The shared data layer (Domain + Database across both hosts) is a deliberate playground tradeoff — the exact coupling a BFF exists to avoid — accepted here because the domain is intentionally small.
Project independence: Client and WebAPI are fully decoupled from each other — neither references the other. 
Both independently reference only Domain and Database, and both query AppDbContext directly, following the same vertical slice pattern. 
The browser talks to Client over HTTP for UI pages and to WebAPI over HTTP for API calls. The two hosts share a schema, not a contract.
Schema ownership: WebAPI owns migrations. Client maps to the existing schema and never migrates. Each host registers its own AppDbContext and connection string.
Security posture: because the UI is server-rendered, no credential or token is ever shipped to browser JS — the same token-isolation property a BFF engineers for SPAs comes for free here. Realized via httpOnly + Secure + SameSite cookie auth, server-side session state, and antiforgery tokens on htmx writes.
htmx: Handles partial page updates — server returns only the HTML fragment that changed, htmx swaps it into the DOM. Supports enhanced navigation for SPA-like feel without full page reloads.
alpine.js: Handles client-side interactivity — dropdowns, toggles, modals, local UI state, form validation feedback. Holds no credentials.

### EF Core Configuration

`IEntityTypeConfiguration<T>` hierarchy mirrors the domain:

`BaseConfiguration<T>` → concrete configs (ShelfConfiguration, BookConfiguration, etc)

`EntityAuditInterceptor` sets `CreatedAt`/`UpdatedAt` on save.

### Response Envelope

- `Response<T>` — wraps data + `Meta` (timestamp, trace ID)
- `PagedResult<T>` — adds pagination metadata; `IQueryable<T>.ToPagedResult()` extension handles sorting and paging
- `GlobalExceptionHandler` — maps `ArgumentException` → 400, unhandled → 500, all as RFC 7807 `ProblemDetails`

### Error Handling

Two mechanisms, split by failure kind:

- **Throw (guard clauses)** — for invariant violations: bad input a correct caller
  would never send (length/null/format checks in `Create` factories, `DateRange`
  start/end). These are bugs. `GlobalExceptionHandler` maps `ArgumentException` → 400,
  unhandled → 500, as RFC 7807 `ProblemDetails`.
- **`Result` / `Result<T>` (`Domain.Common`)** — for *expected* failures with valid
  input. Carries a `ResultError` (an `ErrorCode` + message). Used in two places:
  - **Domain methods** return `Result` for business rules (e.g. `Shelf.AddBook`
    returns `Conflict` when the shelf is full). The rule lives in the aggregate
    because encapsulation makes it unbypassable.
  - **Handlers** return `Result` for *expected failures the domain can't evaluate from
    its own state* — not-found lookups (`ResultError.NotFound`), cross-aggregate rules
    (e.g. uniqueness across shelves), concurrency conflicts, authorization. The handler
    relays the domain's `Result` for in-aggregate rules and persists only on success.
- **Code → HTTP status mapping lives once** in `ApiError.ToProblem(ResultError)`
  (`NotFound`→404, `Conflict`→409, `Validation`→400, else 500). Endpoints fork
  `result.IsSuccess ? ApiResponse.Ok(...) : ApiError.ToProblem(result.Error)` — no
  per-endpoint status switch.

Rule of thumb: expected "no" → return a `Result`; actual bug → throw.

## Conventions

- Snake_case column naming in PostgreSQL
- Nullable audit columns (`CreatedBy`, `UpdatedBy`) — timestamps set by `EntityAuditInterceptor`
- Connection string in `appsettings.Development.json` matches `docker-compose.yml` credentials
- Endpoint groups registered in `EndpointRegistration.cs` via `Map<Feature>Endpoints()` extension methods
- Feature handlers registered in DI via per-feature `Add<Feature>Feature()` extension methods (handlers are `Scoped`)
- C# 13 or newer features used (extension methods new syntax, primary constructors, collection expressions)