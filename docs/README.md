# Atelier Store

.NET 10 Blazor Server app with Tailwind CSS, Auth0, and PostgreSQL.

## Prerequisites

- .NET SDK 10 (pinned in `global.json`)
- Node.js and pnpm (for the Tailwind CLI; install pnpm with `corepack enable` or see <https://pnpm.io/installation>; use pnpm, not npm, which the web project refuses)
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

## Releases

<!-- RELEASES_START -->

| Version | Date | Title | Blog post |
|---------|------|-------|-----------|
| [v0.0.9](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.9) | 2026-09-27 | chore(agents): Add Beast Mode custom agent without gate-bypassing tools | — |
| [v0.0.8](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.8) | 2026-09-27 | chore(vscode): Auto-approve agent git add/commit but not --no-verify | — |
| [v0.0.7](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.7) | 2026-09-27 | test(Web): Add Playwright E2E tests for storefront flows | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-19-test-web-add-playwright-e2e-tests-for-storefront-flows.md) |
| [v0.0.6](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.6) | 2026-09-27 | chore: Ignore Claude Code worktrees | — |
| [v0.0.5](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.5) | 2026-09-27 | test(Web): Add bUnit tests for catalog components and pages | — |
| [v0.0.4](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.4) | 2026-09-26 | test(Web): Add unit tests for stock state, image URLs and return URLs | — |
| [v0.0.3](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.3) | 2026-09-26 | build(tests): Add test foundation and catalog/return-URL seams | — |
| [v0.0.2](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.2) | 2026-09-26 | ci: Lint workflows and shell scripts before push and in CI | — |
| [v0.0.1](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.1) | 2026-09-26 | ci: Add CI, lint, release automation, and git hooks | — |

<!-- RELEASES_END -->

[All releases →](https://github.com/mpaulosky/atelier-store/releases)
