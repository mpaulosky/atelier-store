# Atelier Store

.NET 10 Blazor Server app with Tailwind CSS, Auth0, and PostgreSQL.

## Statuses

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/mpaulosky/atelier-store/blob/main/LICENSE)
[![xUnit Tests](https://img.shields.io/badge/Tests-xUnit-blueviolet?logo=github)](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml)
[![Latest release](https://img.shields.io/github/v/release/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/releases/latest)
[![Codecov](https://img.shields.io/codecov/c/github/mpaulosky/atelier-store?logo=codecov)](https://codecov.io/gh/mpaulosky/atelier-store)
[![Stars](https://img.shields.io/github/stars/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/stargazers)

[![Build and Test Suite](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml)
[![CodeQL](https://github.com/mpaulosky/atelier-store/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/codeql-analysis.yml)
[![.NET code metrics](https://github.com/mpaulosky/atelier-store/actions/workflows/code-metrics.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/code-metrics.yml)

[![Lint Markdown](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-markdown.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-markdown.yml)
[![Lint YAML](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-yaml.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-yaml.yml)

[![Open issues](https://img.shields.io/github/issues/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/issues)
[![Closed issues](https://img.shields.io/github/issues-closed/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/issues?q=is%3Aissue+is%3Aclosed)
[![Open PRs](https://img.shields.io/github/issues-pr/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/pulls)
[![Closed PRs](https://img.shields.io/github/issues-pr-closed/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/pulls?q=is%3Apr+is%3Aclosed)

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
| [v0.0.21](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.21) | 2026-09-28 | ci: Add nightly squad branch and worktree cleanup | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-28-pr-52-ci-add-nightly-squad-branch-and-worktree-cleanup.md) |
| [v0.0.20](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.20) | 2026-09-27 | docs(readme): use an absolute URL for the LICENSE badge link | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-50-docs-readme-use-an-absolute-url-for-the-license-badge-link.md) |
| [v0.0.19](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.19) | 2026-09-27 | docs(readme): regroup status badges | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-47-docs-readme-regroup-status-badges.md) |
| [v0.0.18](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.18) | 2026-09-27 | docs(readme): Add a Statuses badge section | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-44-docs-readme-add-a-statuses-badge-section.md) |
| [v0.0.17](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.17) | 2026-09-27 | ci: Enforce the 80% coverage gate and drop unused Auth0 secrets | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-41-ci-enforce-the-80-coverage-gate-and-drop-unused-auth0-secrets.md) |
| [v0.0.16](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.16) | 2026-09-27 | feat(docs): Add a GitHub Pages site in docs/index.html | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-40-feat-docs-add-a-github-pages-site-in-docs-index-html.md) |
| [v0.0.15](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.15) | 2026-09-27 | docs: Add contributing guide, security policy, references, and code of conduct | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-35-docs-add-contributing-guide-security-policy-references-and-code-of-conduct.md) |
| [v0.0.14](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.14) | 2026-09-27 | fix(Web): Expose the mobile menu toggle as a button | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-36-fix-web-expose-the-mobile-menu-toggle-as-a-button.md) |
| [v0.0.13](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.13) | 2026-09-27 | fix(Web): Require lowercase product and category slugs | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-33-fix-web-require-lowercase-product-and-category-slugs.md) |
| [v0.0.12](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.12) | 2026-09-27 | test(Web): Add Postgres integration tests for catalog and endpoints | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-27-pr-25-test-web-add-postgres-integration-tests-for-catalog-and-endpoints.md) |

<!-- RELEASES_END -->

[All releases →](https://github.com/mpaulosky/atelier-store/releases)
