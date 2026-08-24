# VSliceDDD

Playground for experimenting with vertical slice architecture, backend-for-frontend, and other modern patterns.
A minimal Library (Authors, Books/BookItems, Loans, Members) where the domain stays small on purpose — the
architecture is the experiment, not the features.
Built with .NET 10 (or newer), htmx + alpine.js (planned), and PostgreSQL.

## Tech Stack

- **Backend**: ASP.NET Core Minimal APIs (.NET 10 / C# 13 – or newer)
- **Frontend**: htmx + alpine.js, served via ASP.NET Core Minimal APIs with `RazorComponentResult<T>` (planned — see Client)
- **ORM**: Entity Framework Core 10 (or newer) with Npgsql (PostgreSQL or newer)
- **Validation**: FluentValidation (per-slice validators + endpoint filter)
- **Errors**: ErrorOr (`ErrorOr<T>`) across domain and handlers
- **API docs**: OpenAPI + Scalar UI (Development only, `/scalar`)
- **Observability**: Serilog (console + file sinks), OpenTelemetry tracing (console exporter), health checks
- **Formatting**: CSharpier (dotnet local tool)

## Project Structure

```
src/
Domain/            # Domain model (aggregates, value objects, domain events, shared types)
  Authors/         # Author aggregate (+ AuthorErrors)
  Books/           # Book aggregate + BookItem entity (+ BookErrors)
  Loans/           # Loan aggregate
  Members/         # Member aggregate
  Common/          # Entity/AggregateRoot bases, IDomainEvent, IIdGenerator, DateRange, InvariantViolationException
  Shelves/         # Legacy orphan domain events only (Shelf aggregate removed) — pending cleanup
Database/          # Data access layer (separate project, owns migrations)
  Configurations/  # EF Core entity type configurations (+ DatabaseSettings options)
  Interceptors/    # EntityAuditInterceptor, DomainEventDispatchInterceptor
  Migrations/      # EF Core migrations
  GuidV7IdGenerator.cs
WebAPI/            # ASP.NET Core backend (API endpoints + feature slices)
  Features/        # Vertical slices (endpoints + handlers per feature)
    Authors/       # CreateAuthor, GetAuthorById, DeleteAuthor + Endpoints.cs
    Books/         # CreateBook, GetBookById + Endpoints.cs
    Common/        # ValidationFilter, ProblemLoggingFilter, Problems (error→HTTP), paging types
  Infrastructure/  # GlobalExceptionHandler + Setup/ (modular Add*/Use* builder extensions)
  Program.cs       # Application entry point (composes Setup extensions)
Client/            # ASP.NET Core SSR UI host — skeleton ("Hello World"); Yarp.ReverseProxy referenced in Debug
  Program.cs       # Client entry point
tests/
  Domain.UnitTests/ # Domain unit tests (xUnit + Shouldly)
```

## Build & Run

```sh
# Start PostgreSQL
docker compose up -d

# Restore dotnet local tools (csharpier)
dotnet tool restore

# Run the API
dotnet run --project src/WebAPI

# Run the Client (SSR frontend — currently a skeleton)
dotnet run --project src/Client

# Build the solution (VSliceDDD.slnx)
dotnet build

# Add a migration (migrations live in the Database project)
dotnet ef migrations add <Name> --project src/Database --startup-project src/WebAPI

# Apply migrations
dotnet ef database update --project src/Database --startup-project src/WebAPI

# Run tests
dotnet test
```

- Scalar API reference: `http://localhost:5243/scalar` (Development only)
- Health checks: `/_health` (includes EF Core DbContext check)

## Architecture

The architecture is **Vertical Slice** as the primary organising principle, with selected DDD building blocks where they
add value. There is no repository pattern — EF Core's `DbContext` is used directly as the data access layer.

### Vertical Slices

Features live in `Features/<Feature>/`. Each slice owns its endpoint, request/response types, validation, and handler
logic. Slices are allowed to reach directly into `AppDbContext` and the domain model — no intermediate repository or
service layer between the handler and EF Core.

A slice typically contains:

- `Endpoints.cs` — exposes `Map<Feature>Endpoints()` (registers the feature's routes under the `/api` group) and
  `Add<Feature>Feature()` (registers the feature's handlers in DI as `Scoped`)
- One file per operation (e.g. `CreateAuthor.cs`, `GetBookById.cs`) — a static class containing:
  - nested `Request` / `Response` records
  - a FluentValidation `Validator` for the request (where applicable)
  - a `Handler` class using primary-constructor DI (`AppDbContext`, `IIdGenerator`, …) with a `HandleAsync()` method
  - a static `Endpoint` delegate returning typed `Results<...>`, forking on
    `result.IsError ? Problems.From(result.FirstError) : <success result>`

Shared endpoint behaviour lives in `Features/Common`:

- `ValidationFilter<T>` — runs the FluentValidation validator for `T` before the endpoint; 400 on failure
- `ProblemLoggingFilter` — applied to the whole `/api` group; logs 409/401/403 problem responses with their error code
- `Problems` — central `Error` → HTTP mapping (see Error Handling)
- Paging: `ListRequest` (Page/PageSize), `PagedResult<T>` + `ToPagedResultAsync()`, `PagedResponse<T>` with
  `PaginationMetadata`/`PaginationLinks`

### DDD Building Blocks

DDD is used selectively within the domain model, not as a full architecture:

- **Aggregate hierarchy**: `Entity` → `AggregateRoot`
- **Factory methods**: entities use static `Create()` — no public constructors
- **IDs**: generated outside the aggregate via `IIdGenerator` (GuidV7, registered as singleton) and passed into
  `Create()` — handlers own ID generation
- **Encapsulation**: `private set` / `protected init`; mutable collections exposed only as `IReadOnlyList<T>`
- **Value objects**: e.g. `DateRange` (C# records)
- **Result type**: `ErrorOr` / `ErrorOr<T>` (`Error` carries a string code + description + `ErrorType`). For outcomes
  that can legitimately fail — in-aggregate business rules (domain) and failures the aggregate can't evaluate from its
  own state (handlers: not-found, cross-aggregate rules, concurrency, authorization). Mapped to HTTP centrally (see
  Error Handling).
- **Per-aggregate error factories**: static classes like `BookErrors` / `AuthorErrors` returning
  `Error.NotFound(...)` / `Error.Conflict(...)` with stable string codes (e.g. `"Book.DuplicateIsbn"`)
- **Domain events**: `IDomainEvent` with a collection on `AggregateRoot` (`Raise`/`ClearDomainEvents`). Dispatched
  post-save by `DomainEventDispatchInterceptor` (stamps `OccurredAt`; currently logs to console — real handlers TBD)

No repository pattern. No application service layer. Handlers in feature slices work directly with `AppDbContext`.

### Business Logic Placement

- **In-aggregate rules** (enforceable from one aggregate's own state, e.g. "no duplicate barcode within a book") live
  in the aggregate — never duplicated or reached-around in a handler. The aggregate is the rule's guardian
  (encapsulation: private collections, `internal` mutators).
- **Cross-aggregate rules, orchestration, and decisions needing outside data** (existence of a related aggregate,
  uniqueness across aggregates, multi-step workflows, anything requiring the DB, clock, current user, or another
  service) live in the handler — or a domain service the handler calls. The domain can't reach these.
- Handlers must never re-implement or bypass an in-aggregate rule (e.g. checking `book.BookItems` barcodes directly
  instead of letting `AddCopy` decide). That drifts.

Rule of thumb: can one aggregate decide it from its own state? → domain. Does it need other aggregates, the DB, or the
request context? → handler.

### Frontend (Client Project — SSR Web Host)

> **Planned — not yet implemented.** Client is currently a skeleton Minimal APIs host ("Hello World") with
> `Yarp.ReverseProxy` referenced in Debug configuration only. Everything below is design intent.

The Client project is a separate ASP.NET Core Minimal APIs server running on its own port.
It serves HTML pages rendered via `RazorComponentResult<T>` — Razor components rendered to static HTML (no SignalR, no
persistent connection).
It is a server-side-rendered (SSR) UI host, not a BFF: it does not proxy or call WebAPI.
It queries the database directly through its own `AppDbContext`, sitting at the same tier as WebAPI rather than in
front of it (Browser → Client → DB, parallel to Browser → WebAPI → DB).
A true BFF would source its data by calling a backend API and own no schema; Client owns data access, so the BFF label
does not apply.
The shared data layer (Domain + Database across both hosts) is a deliberate playground tradeoff — the exact coupling a
BFF exists to avoid — accepted here because the domain is intentionally small.
Project independence: Client and WebAPI are fully decoupled from each other — neither references the other.
Both independently reference only Domain and Database, and both query `AppDbContext` directly, following the same
vertical slice pattern.
The browser talks to Client over HTTP for UI pages and to WebAPI over HTTP for API calls. The two hosts share a schema,
not a contract.
Schema ownership: WebAPI owns migrations (via the Database project). Client maps to the existing schema and never
migrates. Each host registers its own `AppDbContext` and connection string.
Security posture: because the UI is server-rendered, no credential or token is ever shipped to browser JS — the same
token-isolation property a BFF engineers for SPAs comes for free here. Realized via httpOnly + Secure + SameSite cookie
auth, server-side session state, and antiforgery tokens on htmx writes.
htmx: Handles partial page updates — server returns only the HTML fragment that changed, htmx swaps it into the DOM.
Supports enhanced navigation for SPA-like feel without full page reloads.
alpine.js: Handles client-side interactivity — dropdowns, toggles, modals, local UI state, form validation feedback.
Holds no credentials.

### EF Core Configuration

`IEntityTypeConfiguration<T>` hierarchy mirrors the domain:

`BaseConfiguration<T>` → `AggregateRootConfiguration<T>` → concrete configs (AuthorConfiguration, BookConfiguration,
BookItemConfiguration, LoanConfiguration, MemberConfiguration)

Interceptors (registered Scoped, resolved per-DbContext):

- `EntityAuditInterceptor` — sets `CreatedAt`/`UpdatedAt` on save via `TimeProvider`
- `DomainEventDispatchInterceptor` — dispatches aggregate domain events after successful save

### Response Shapes

- No global response envelope — endpoints return plain DTOs (`Ok`, `CreatedAtRoute`, `NoContent`) or `ProblemDetails`
- `PagedResult<T>` / `PagedResponse<T>` — paging metadata + links; `IQueryable<T>.ToPagedResultAsync(ListRequest)`
  handles paging (default page 1, size 20, max page size 100)
- `GlobalExceptionHandler` — RFC 7807 `ProblemDetails` for unhandled exceptions (see Error Handling)

### Error Handling

Two mechanisms, split by failure kind:

- **Throw (invariants)** — for post-condition/state-integrity checks: the aggregate verifies its own state is as
  expected after an operation (and guards in `Create` factories / mutators). A violated invariant means the code is
  wrong — these are bugs, not bad requests. `InvariantViolationException` is the dedicated type for these
  (plain `ArgumentException` guards also exist). `GlobalExceptionHandler` maps unhandled exceptions → 500 as RFC 7807
  `ProblemDetails` (plus `BadHttpRequestException` → 400, `DbUpdateConcurrencyException` → 409).
- **`ErrorOr` / `ErrorOr<T>`** — for *expected* failures with valid input. Used in two places:
  - **Domain methods** return `ErrorOr<...>` for business rules (e.g. `Book.AddCopy` returns `Conflict` on a duplicate
    barcode). The rule lives in the aggregate because encapsulation makes it unbypassable.
  - **Handlers** return `ErrorOr<...>` for *expected failures the domain can't evaluate from its own state* —
    not-found lookups (`BookErrors.NotFound`), cross-aggregate rules (e.g. author must exist before a book is
    created), concurrency conflicts, authorization. The handler relays the domain's errors for in-aggregate rules and
    persists only on success.
- **Request validation** — FluentValidation validators run in `ValidationFilter<T>` before the endpoint; failures
  return 400 `HttpValidationProblemDetails` (stamped with code `Request.ValidationFailed` + `traceId`).
- **Error type → HTTP status mapping lives once** in `Problems.From(Error)`
  (`NotFound`→404, `Conflict`→409, `Validation`→400, `Unauthorized`→401, `Forbidden`→403, else 500). Server errors
  (≥500) return a generic detail message; all problems are stamped with `traceId` + `code` extensions. Endpoints fork
  `result.IsError ? Problems.From(result.FirstError) : ...` — no per-endpoint status switch.
- **Database constraint violations** — unique-constraint violations surface as 409
  (e.g. `CreateBook` catches `UniqueConstraintException` → `BookErrors.DuplicateIsbn`) via
  `EntityFrameworkCore.Exceptions.PostgreSQL` (`UseExceptionProcessor()` in `DatabaseSetup` translates Npgsql 23505
  → `UniqueConstraintException`; logged as 409 via `ProblemLoggingFilter`).

Rule of thumb: expected "no" → return an `Error`; broken state/invariant → throw.

## Conventions

- Snake_case column naming in PostgreSQL
- Audit columns are timestamps only (`CreatedAt`, `UpdatedAt`) — set by `EntityAuditInterceptor`
- Connection string lives under the `Database:DefaultConnection` section (validated `DatabaseSettings` options) and
  matches `docker-compose.yml` credentials
- Endpoint groups registered in `EndpointRegistration.cs` via `Map<Feature>Endpoints()` extension methods under the
  `/api` route group
- Feature handlers registered in DI via per-feature `Add<Feature>Feature()` extension methods (handlers are `Scoped`)
- All projects: `TreatWarningsAsErrors`, `Nullable enable`, latest-recommended analyzers, code style enforced in build
- Formatting via CSharpier (`dotnet csharpier .`)
- Modern C# features used (primary constructors, collection expressions, source-generated logging)
