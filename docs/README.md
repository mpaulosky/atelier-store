# Atelier Store

.NET 10 Blazor Server app with Tailwind CSS, Auth0, and PostgreSQL.

## Statuses

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/mpaulosky/atelier-store/blob/main/LICENSE)
[![xUnit Tests](https://img.shields.io/badge/Tests-xUnit-blueviolet?logo=github)](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml)
[![Latest release](https://img.shields.io/github/v/release/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/releases/latest)
[![Codecov](https://img.shields.io/codecov/c/github/mpaulosky/atelier-store?logo=codecov)](https://codecov.io/gh/mpaulosky/atelier-store)
[![Stars](https://img.shields.io/github/stars/mpaulosky/atelier-store)](https://github.com/mpaulosky/atelier-store/stargazers)

[![Build and Test Suite](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/mpaulosky/atelier-store/actions/workflows/ci.yml)
[![CodeQL](https://github.com/mpaulosky/atelier-store/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/codeql-analysis.yml)
[![.NET code metrics](https://github.com/mpaulosky/atelier-store/actions/workflows/code-metrics.yml/badge.svg)](https://github.com/mpaulosky/atelier-store/actions/workflows/code-metrics.yml)

[![Lint Markdown](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-markdown.yml/badge.svg?branch=main)](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-markdown.yml)
[![Lint YAML](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-yaml.yml/badge.svg?branch=main)](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-yaml.yml)
[![Lint Actions](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-actions.yml/badge.svg?branch=main)](https://github.com/mpaulosky/atelier-store/actions/workflows/lint-actions.yml)

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
| [v0.0.44](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.44) | 2026-10-05 | chore: Re-apply the repo-ci-baseline Template | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-10-05-pr-109-chore-re-apply-the-repo-ci-baseline-template.md) |
| [v0.0.43](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.43) | 2026-10-05 | chore: Re-apply the repo-ci-baseline Template | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-10-05-pr-107-chore-re-apply-the-repo-ci-baseline-template.md) |
| [v0.0.42](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.42) | 2026-10-04 | chore: Standardize on the repo-ci-baseline Template | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-10-04-pr-103-chore-standardize-on-the-repo-ci-baseline-template.md) |
| [v0.0.41](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.41) | 2026-09-30 | fix(ci): Escape logged paths; wait for a new Release to be listed | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-101-fix-ci-escape-logged-paths-wait-for-a-new-release-to-be-listed.md) |
| [v0.0.40](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.40) | 2026-09-30 | ci: Drop the build cache no job restores | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-99-ci-drop-the-build-cache-no-job-restores.md) |
| [v0.0.39](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.39) | 2026-09-30 | ci: Skip the build and tests for docs-only pull requests | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-95-ci-skip-the-build-and-tests-for-docs-only-pull-requests.md) |
| [v0.0.38](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.38) | 2026-09-30 | fix(release): Decide a draft Release's body in a tested helper | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-94-fix-release-decide-a-draft-release-s-body-in-a-tested-helper.md) |
| [v0.0.37](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.37) | 2026-09-30 | ci(hooks): Adopt the shared branch-name standard | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-92-ci-hooks-adopt-the-shared-branch-name-standard.md) |
| [v0.0.36](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.36) | 2026-09-30 | ci(release): Release merged PRs one at a time, in merge order | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-90-ci-release-release-merged-prs-one-at-a-time-in-merge-order.md) |
| [v0.0.35](https://github.com/mpaulosky/atelier-store/releases/tag/v0.0.35) | 2026-09-30 | ci(hooks): Lint the staged Markdown, not the working copy | [Post](https://github.com/mpaulosky/atelier-store/blob/main/docs/blogs/2026-09-30-pr-87-ci-hooks-lint-the-staged-markdown-not-the-working-copy.md) |

<!-- RELEASES_END -->

[All releases →](https://github.com/mpaulosky/atelier-store/releases)
