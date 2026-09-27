# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Atelier Store: a .NET 10 Blazor Web App with Tailwind CSS v4, Auth0 login, and PostgreSQL via EF Core (Npgsql).
The SDK is pinned in `global.json`. `AtelierStore.slnx` holds one app project, `src/AtelierStore.Web`, which has its own `CLAUDE.md` covering the web project.
It also holds four test projects under `tests/` (see **Tests**).

**Catalog data** lives in Postgres: `categories` 1─< `products` 1─1 `product_stock` (entities in `src/AtelierStore.Web/Data/`, snake_case names via `EFCore.NamingConventions`).
The starter catalog is seeded through `HasData` in `Data/CatalogSeedData.cs`, so changing it means adding a migration.
Pages read products only through `Catalog/ProductCatalog.cs`, which projects into the `Catalog.Product` view record that the components render.
Product and category slugs must be lowercase: check constraints reject anything else, because `ProductCatalog` lowercases the slug it looks up.
Featured collections are editorial content and are still hard-coded in `Catalog/CatalogModels.cs`.

## Commands

```bash
cp .env.example .env                  # fill in Auth0 values
docker compose up -d                  # local Postgres 17 on :5432
dotnet tool restore                   # installs dotnet-ef (tool manifest: dotnet-tools.json)
git config core.hooksPath .github/hooks  # enable the repo git hooks (one-time, per clone)
dotnet build AtelierStore.slnx
dotnet run --project src/AtelierStore.Web --launch-profile https   # https://localhost:7207
dotnet test --project tests/AtelierStore.Web.Tests.Unit            # one test project (MTP runner)

# EF Core migrations (need .env present, because design time runs Program.cs)
dotnet ef migrations add <Name> --project src/AtelierStore.Web
dotnet ef database update --project src/AtelierStore.Web
```

## Tests

Every project under `tests/` is an xUnit v3 executable on Microsoft Testing Platform (`global.json` sets the runner).
`tests/Directory.Build.props` gives them all xUnit, FluentAssertions, NSubstitute, and the MTP code-coverage extension, so a test csproj only adds what is specific to it.

| Project | What it covers | Needs |
| --- | --- | --- |
| `AtelierStore.Web.Tests.Unit` | Plain classes (catalog, account helpers) | nothing |
| `AtelierStore.Web.Tests.Bunit` | Razor components rendered with bUnit, catalog faked via `IProductCatalog` | nothing |
| `AtelierStore.Web.Tests.Integration` | `ProductCatalog`, seed data, DB constraints, and HTTP endpoints via `WebApplicationFactory` | Docker (Testcontainers Postgres, reset with Respawn) |
| `AtelierStore.Web.Tests.E2E` | Storefront flows in a real browser via Playwright | Docker, plus Chromium (see below) |

Before the first E2E run, build the project and run `pwsh bin/<Configuration>/net10.0/playwright.ps1 install chromium` in its folder.
Integration and E2E fixtures point `ConnectionStrings__Default` at the container, set fake `Auth0__*` values, and swap in a fake auth scheme.
Tests therefore never touch the real Auth0 tenant or the developer database, and need no `.env`.

CI (`.github/workflows/ci.yml`) discovers every csproj under `tests/` and runs each as its own matrix job.
A **Coverage Analysis** job merges their Cobertura reports and fails if line coverage is below 80%.
Locally, `scripts/gate.sh` (run by the pre-push hook) runs each test project in turn.

## Solution-wide conventions

- **NuGet versions are centrally managed** in `Directory.Packages.props` (with transitive pinning on).
  A `<PackageReference>` in a csproj must not have a `Version`.
  Add a `<PackageVersion>` entry to `Directory.Packages.props` instead.
  `dotnet add package` does this automatically.
  Tool versions (`dotnet-ef`) live separately in `dotnet-tools.json`.
- **Node packages use pnpm, never npm or npx.**
  The only Node project is `src/AtelierStore.Web` (the Tailwind CLI), pinned via `packageManager` in its `package.json`.
  Use `pnpm add`, `pnpm install`, `pnpm run`, and `pnpm dlx` (not `npx`), and commit `pnpm-lock.yaml`; never add a `package-lock.json`.
  `engines.npm` plus `engine-strict=true` in the project's `.npmrc` make `npm install` fail on purpose.
- **Configuration** comes from `.env` at the repo root (see `.env.example`) plus real environment variables. Keys use `__` for nesting (`Auth0__Domain` → `Auth0:Domain`).
- **Code style**: `.editorconfig` asks for **tabs (width 2)** in C#, Razor, CSS, JSON, and JS, plus LF line endings and `System` usings sorted first.
  Explicit types are preferred over `var` (`csharp_style_var_* = false`).
  The scaffolded files currently use spaces; follow `.editorconfig` for new code.
