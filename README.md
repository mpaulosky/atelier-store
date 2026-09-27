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
| `docs/index.html` | GitHub Pages site; its blog cards and releases table are refreshed by the release workflow |

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
| ------- | ---- | ----- | --------- |
| [v0.0.15](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.15) | 2026-09-27 | docs: Add contributing guide, security policy, references, and code of conduct | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-35-docs-add-contributing-guide-security-policy-references-and-code-of-conduct.md) |
| [v0.0.14](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.14) | 2026-09-27 | fix(Web): Expose the mobile menu toggle as a button | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-36-fix-web-expose-the-mobile-menu-toggle-as-a-button.md) |
| [v0.0.13](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.13) | 2026-09-27 | fix(Web): Require lowercase product and category slugs | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-33-fix-web-require-lowercase-product-and-category-slugs.md) |
| [v0.0.12](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.12) | 2026-09-27 | test(Web): Add Postgres integration tests for catalog and endpoints | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-25-test-web-add-postgres-integration-tests-for-catalog-and-endpoints.md) |
| [v0.0.11](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.11) | 2026-09-27 | chore(ci): Run code metrics on manual dispatch only | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-27-chore-ci-run-code-metrics-on-manual-dispatch-only.md) |
| [v0.0.10](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.10) | 2026-09-27 | fix(ci): Write markdownlint-clean table separators in release tables | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-28-fix-ci-write-markdownlint-clean-table-separators-in-release-tables.md) |
| [v0.0.9](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.9) | 2026-09-27 | chore(agents): Add Beast Mode custom agent without gate-bypassing tools | — |
| [v0.0.8](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.8) | 2026-09-27 | chore(vscode): Auto-approve agent git add/commit but not --no-verify | — |
| [v0.0.7](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.7) | 2026-09-27 | test(Web): Add Playwright E2E tests for storefront flows | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-19-test-web-add-playwright-e2e-tests-for-storefront-flows.md) |
| [v0.0.6](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.6) | 2026-09-27 | chore: Ignore Claude Code worktrees | — |

<!-- RELEASES_END -->

[All releases →](https://github.com/mpaulosky/atelier-store/releases)
