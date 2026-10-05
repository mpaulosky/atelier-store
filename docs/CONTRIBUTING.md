# Contributing to Atelier Store

Thanks for helping out.
Atelier Store is a .NET 10 Blazor Web App (Interactive Server) with Tailwind CSS v4, Auth0 login, and PostgreSQL through EF Core.
This guide covers how to set up and what the repo checks for.
How a change gets from a branch to `main` (branches, worktrees, commits, PRs, review, merging and releases) is in [PROCESS.md](PROCESS.md).

## Table of contents

- [Code of conduct](#code-of-conduct)
- [Ways to contribute](#ways-to-contribute)
- [Set up your environment](#set-up-your-environment)
- [Project layout](#project-layout)
- [Make a change](#make-a-change)
  - [Branches and commits](#branches-and-commits)
  - [Code style](#code-style)
  - [Database changes](#database-changes)
- [Tests](#tests)
- [Local checks](#local-checks)
- [Pull requests](#pull-requests)

## Code of conduct

This project follows the [Contributor Covenant](CODE_OF_CONDUCT.md).
Report unacceptable behavior to the contact listed there.

## Ways to contribute

- **Report a bug:** open an issue with the `bug` label.
  Include steps to reproduce, what you expected, and what happened.
- **Suggest an improvement:** open an issue with the `enhancement` label.
- **Fix or build something:** pick an issue, or open one first, so two people don't work on the same thing.
  Every code change should link to an issue.
- **Improve the docs:** fixes to this guide, the [README](../README.md), or anything under `docs/` are welcome.

Please don't report security vulnerabilities in a public issue.
Use the repository's **Security** tab to report one privately.

## Set up your environment

You need:

- The .NET 10 SDK (the exact version is pinned in `global.json`)
- Node.js and pnpm, for the Tailwind CLI.
  Install pnpm with `corepack enable`.
- Docker, for the local database and for the integration and E2E tests
- An Auth0 tenant with a Regular Web Application, if you want to sign in locally

Then, from the repository root:

```bash
cp .env.example .env                     # fill in your Auth0 values
docker compose up -d                     # Postgres 17 on port 5432
dotnet tool restore                      # installs dotnet-ef
git config core.hooksPath .github/hooks  # turn on the repo's git hooks (once per clone)
dotnet ef database update --project src/AtelierStore.Web
dotnet run --project src/AtelierStore.Web --launch-profile https
```

The app runs at <https://localhost:7207>.
In your Auth0 application, set the Allowed Callback URL to `https://localhost:7207/callback` and the Allowed Logout URL to `https://localhost:7207/`.

Configuration comes from `.env` plus real environment variables, and keys use `__` for nesting (`Auth0__Domain` becomes `Auth0:Domain`).
Never commit `.env`.

Use pnpm for anything Node-related: `pnpm install`, `pnpm add`, `pnpm run`, and `pnpm dlx` instead of `npx`.
The web project refuses `npm install` on purpose.

## Project layout

```text
src/AtelierStore.Web/                  The Blazor Web App
  Program.cs                           All service wiring and the /account/* and /health endpoints
  Account/                             Sign-in helpers, such as the return URL check
  Catalog/                             Product queries (ProductCatalog) and the view models pages render
  Components/                          Razor components, layouts, and pages
  Data/                                EF Core context, catalog entities, and seed data
  Migrations/                          EF Core migrations
  Styles/app.css                       Tailwind entry point, compiled to wwwroot/app.css on build
tests/
  AtelierStore.Web.Tests.Unit/         Unit tests
  AtelierStore.Web.Tests.Bunit/        Blazor component tests (bUnit)
  AtelierStore.Web.Tests.Integration/  Tests against real Postgres (Testcontainers)
  AtelierStore.Web.Tests.E2E/          Browser tests (Playwright)
.github/
  workflows/                           CI, linting, CodeQL, releases, and PR auto-merge
  hooks/                               The git hooks behind the local checks
  instructions/                        Coding, Blazor, Markdown, and commit guidelines
scripts/gate.sh                        The checks the pre-push hook runs
CONTEXT.md                             Domain language
```

`CLAUDE.md` at the root and in `src/AtelierStore.Web/` explains the architecture in more detail.

## Make a change

### Branches and commits

Work on a branch named to the standard (such as `fix/51-cart-total`), in its own worktree under `../atelier-store-worktrees/`.
Commits and PR titles use the `<type>(<scope>): <Summary>` format from [git-commit-instructions.md](../.github/instructions/git-commit-instructions.md).
[PROCESS.md](PROCESS.md#branches-and-worktrees) has the branch names, the worktree commands and the commit rules.
Scopes here are the affected project or area, such as `Web`, `Catalog`, `Data`, or `ci`.

### Code style

`.editorconfig` sets the formatting, and the files in [.github/instructions/](../.github/instructions/) cover .NET, Blazor, and Markdown conventions.
The main rules:

- Indent with tabs (width 2) in C#, Razor, CSS, JSON, and JS, and use LF line endings.
- Prefer explicit types over `var`.
- Sort `System` usings first.
- Add comments that explain *why*, not *what*.

NuGet versions are managed centrally.
Add a `<PackageVersion>` entry to `Directory.Packages.props` and leave `Version` off the `<PackageReference>` in the project file.
`dotnet add package` does this for you.

### Database changes

The catalog schema and its seed data live in `src/AtelierStore.Web/Data/`.
If you change an entity, the model configuration in `AppDbContext`, or the seed data in `CatalogSeedData.cs`, add a migration:

```bash
dotnet ef migrations add <Name> --project src/AtelierStore.Web
```

The EF tools run `Program.cs` at design time, so they need your `.env`.
Commit the migration, its `.Designer.cs` file, and the updated model snapshot together.

### The project site

`docs/index.html` is the GitHub Pages site, served from `main` under `/docs`.
The release workflow rewrites its blog cards and releases table on every release, between the `BLOGS_HTML` and `RELEASES_HTML` markers, so don't edit inside them.
The rest of the page is hand-written, and its hero, setup steps, project layout, and endpoints repeat the README.
When you change those parts of the README, update the page to match.

## Tests

Every code change needs tests, and a bug fix should include a test that fails without the fix.
The tests use xUnit v3, FluentAssertions, and NSubstitute.
Choose the project that fits the change:

- **Unit:** logic that doesn't need a database or a browser.
- **bUnit:** rendering and interaction of Razor components.
- **Integration:** EF Core queries, database constraints, migrations, and HTTP endpoints, against a real Postgres container.
- **E2E:** full storefront flows in a real browser.

Every test method has `// Arrange`, `// Act`, and `// Assert` comments, each on its own line.

Run the tests with:

```bash
dotnet build AtelierStore.slnx -c Release
dotnet test --project tests/AtelierStore.Web.Tests.Unit -c Release --no-build
```

Swap in another project folder to run its tests.
The integration and E2E tests start their own Postgres container, so Docker must be running.
Before the first E2E run, install the Playwright browser from the build output:

```bash
pwsh tests/AtelierStore.Web.Tests.E2E/bin/Release/net10.0/playwright.ps1 install chromium
```

If a build fails with Razor errors in files you didn't touch, such as `Parameter` or `EditorRequired` not being found, run `dotnet build-server shutdown` and build again.

## Local checks

With the hooks turned on (`git config core.hooksPath .github/hooks`), git runs the same checks as CI before your code leaves your machine:

- **On commit:** markdownlint checks the staged Markdown files.
- **On push:** the branch name is checked, then `scripts/gate.sh` lints the Markdown, YAML, workflow, and shell files you changed.
  It then builds the solution with warnings treated as errors and runs every test project.

The push check refuses to run if you have uncommitted or untracked files, because it can only vouch for what is in the commit you push.
Commit your work or set it aside first.
You can also run `scripts/gate.sh` by hand at any time.

Don't skip the hooks with `--no-verify`.
If a check fails, fix the cause.

## Pull requests

Push your branch and open a pull request against `main` using the template.
Every PR is reviewed by Copilot on each push, and merges once its required checks pass and every review thread is resolved:
a same-repo PR merges on its own, and the maintainer merges a fork's.
[PROCESS.md](PROCESS.md#checks-review-and-merging) covers the PR description, the required checks, review, merging and releases.

Thanks for contributing!
