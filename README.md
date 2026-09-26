# Atelier Store

.NET 10 Blazor Server app with Tailwind CSS, Auth0, and PostgreSQL.

## Prerequisites

- .NET SDK 10 (pinned in `global.json`)
- Node.js and pnpm (for the Tailwind CLI; install pnpm with `npm i -g pnpm` or `corepack enable`)
- Docker (for local Postgres)

## Setup

```bash
cp .env.example .env          # then fill in the Auth0 values
docker compose up -d          # start Postgres
dotnet tool restore           # installs dotnet-ef
git config core.hooksPath .github/hooks  # enable the repo git hooks (one-time, per clone)
dotnet ef database update --project src/AtelierStore.Web   # create the catalog tables and seed data
dotnet run --project src/AtelierStore.Web --launch-profile https
```

The app runs at <https://localhost:7207>. In your Auth0 Regular Web Application, set:

- Allowed Callback URLs: `https://localhost:7207/callback`
- Allowed Logout URLs: `https://localhost:7207/`

## Layout

| Path | Purpose |
| --- | --- |
| `src/AtelierStore.Web/Program.cs` | Service wiring: Auth0, EF Core/Npgsql, health checks |
| `src/AtelierStore.Web/Data/` | EF Core context, catalog entities (categories, products, product stock) and seed data |
| `src/AtelierStore.Web/Migrations/` | EF Core migrations; `InitialCatalog` creates and seeds the catalog tables |
| `src/AtelierStore.Web/Catalog/ProductCatalog.cs` | Storefront product queries used by the pages |
| `src/AtelierStore.Web/Styles/app.css` | Tailwind entry point, compiled to `wwwroot/app.css` on build |
| `.github/workflows/` | CI (build, tests, lint incl. actionlint/zizmor/shellcheck, CodeQL for C# and Actions), release tags and blog posts, Dependabot and PR auto-merge |
| `.github/hooks/`, `scripts/gate.sh` | Git hooks: Markdown lint on commit; branch naming, lint (Markdown, YAML, workflows, shell), build and tests on push |

## Endpoints

- `/account/login`, `/account/logout`: Auth0 sign-in/out
- `/health`: checks the Postgres connection

## Tailwind

The build runs `pnpm install` (first time only) and `pnpm run css:build` automatically. During UI work, run
`pnpm run css:watch` in `src/AtelierStore.Web`. Pass `-p:SkipTailwind=true` to skip the CSS step.

## Configuration

Settings come from `appsettings.json`, then `.env` / environment variables (`Auth0__Domain` → `Auth0:Domain`).
The app fails at startup if `Auth0:Domain`, `Auth0:ClientId`, or `ConnectionStrings:Default` is missing.
