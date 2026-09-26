# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Atelier Store: a .NET 10 Blazor Web App with Tailwind CSS v4, Auth0 login, and PostgreSQL via EF Core (Npgsql).
The SDK is pinned in `global.json`. `AtelierStore.slnx` currently holds one project, `src/AtelierStore.Web`, which has its own `CLAUDE.md` covering the web project.
The repo is at an early scaffold stage, and there is no test project.

**Catalog data** lives in Postgres: `categories` 1─< `products` 1─1 `product_stock` (entities in `src/AtelierStore.Web/Data/`, snake_case names via `EFCore.NamingConventions`).
The starter catalog is seeded through `HasData` in `Data/CatalogSeedData.cs`, so changing it means adding a migration.
Pages read products only through `Catalog/ProductCatalog.cs`, which projects into the `Catalog.Product` view record that the components render.
Featured collections are editorial content and are still hard-coded in `Catalog/CatalogModels.cs`.

## Commands

```bash
cp .env.example .env                  # fill in Auth0 values
docker compose up -d                  # local Postgres 17 on :5432
dotnet tool restore                   # installs dotnet-ef (tool manifest: dotnet-tools.json)
git config core.hooksPath .github/hooks  # enable the repo git hooks (one-time, per clone)
dotnet build AtelierStore.slnx
dotnet run --project src/AtelierStore.Web --launch-profile https   # https://localhost:7207

# EF Core migrations (need .env present, because design time runs Program.cs)
dotnet ef migrations add <Name> --project src/AtelierStore.Web
dotnet ef database update --project src/AtelierStore.Web
```

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
