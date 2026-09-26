# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Atelier Store: a .NET 10 Blazor Web App with Tailwind CSS v4, Auth0 login, and PostgreSQL via EF Core (Npgsql). The SDK is pinned in `global.json`. `AtelierStore.slnx` currently holds one project, `src/AtelierStore.Web`, which has its own `CLAUDE.md` covering the web project. The repo is at an early scaffold stage: `AppDbContext` has no entities or migrations yet, and there is no test project.

## Commands

```bash
cp .env.example .env                  # fill in Auth0 values
docker compose up -d                  # local Postgres 17 on :5432
dotnet tool restore                   # installs dotnet-ef (tool manifest: dotnet-tools.json)
dotnet build AtelierStore.slnx
dotnet run --project src/AtelierStore.Web --launch-profile https   # https://localhost:7207

# EF Core migrations (need .env present, because design time runs Program.cs)
dotnet ef migrations add <Name> --project src/AtelierStore.Web
dotnet ef database update --project src/AtelierStore.Web
```

## Solution-wide conventions

- **NuGet versions are centrally managed** in `Directory.Packages.props` (with transitive pinning on). A `<PackageReference>` in a csproj must not have a `Version`. Add a `<PackageVersion>` entry to `Directory.Packages.props` instead. `dotnet add package` does this automatically. Tool versions (`dotnet-ef`) live separately in `dotnet-tools.json`.
- **Configuration** comes from `.env` at the repo root (see `.env.example`) plus real environment variables. Keys use `__` for nesting (`Auth0__Domain` → `Auth0:Domain`).
- **Code style**: `.editorconfig` asks for **tabs (width 2)** in C#, Razor, CSS, JSON, and JS, plus LF line endings and `System` usings sorted first. Explicit types are preferred over `var` (`csharp_style_var_* = false`). The scaffolded files currently use spaces; follow `.editorconfig` for new code.
