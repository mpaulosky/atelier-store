# References Used in Atelier Store

The libraries, tools, and services this project is built with.
Exact NuGet versions are in `Directory.Packages.props`, the .NET SDK is pinned in `global.json`, and the Node packages are in `src/AtelierStore.Web/package.json`.

## Platform and frameworks

- [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) – The runtime and SDK
- [C# 14](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14) – The language version
- [Blazor Web App](https://learn.microsoft.com/aspnet/core/blazor/) – The UI framework, using the Interactive Server render mode
- [ASP.NET Core health checks](https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks) – The `/health` endpoint, including a database check
- [Tailwind CSS v4](https://tailwindcss.com/docs) – Styling, compiled by the Tailwind CLI as part of the build

## Authentication

- [Auth0](https://auth0.com/docs) – The identity provider (OpenID Connect)
- [Auth0 ASP.NET Core Authentication SDK](https://github.com/auth0/auth0-aspnetcore-authentication) – Sign-in, sign-out, and the cookie session

## Data

- [PostgreSQL 17](https://www.postgresql.org/docs/17/) – The database, run locally with Docker Compose
- [Entity Framework Core 10](https://learn.microsoft.com/ef/core/) – The ORM, with code-first migrations and `HasData` seed data
- [Npgsql EF Core provider](https://www.npgsql.org/efcore/) – Connects EF Core to PostgreSQL
- [EFCore.NamingConventions](https://github.com/efcore/EFCore.NamingConventions) – Maps entities to `snake_case` table and column names
- [dotnet-ef](https://learn.microsoft.com/ef/core/cli/dotnet) – The EF Core command-line tool, installed with `dotnet tool restore`

## Configuration

- [DotNetEnv](https://github.com/tonerdo/dotnet-env) – Loads the local `.env` file into environment variables at startup

## Testing

- [xUnit v3](https://xunit.net/) – The test framework for all four test projects
- [Microsoft Testing Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro) – The test runner behind `dotnet test`
- [FluentAssertions](https://fluentassertions.com/) – Assertions
- [NSubstitute](https://nsubstitute.github.io/) – Test doubles, with [NSubstitute.Analyzers](https://github.com/nsubstitute/NSubstitute.Analyzers) to catch misuse
- [bUnit](https://bunit.dev/) – Blazor component tests
- [Microsoft.AspNetCore.Mvc.Testing](https://learn.microsoft.com/aspnet/core/test/integration-tests) – `WebApplicationFactory`: in memory for integration tests, on real Kestrel for E2E
- [Testcontainers for .NET](https://dotnet.testcontainers.org/) – Starts a throwaway PostgreSQL container for the integration and E2E tests
- [Respawn](https://github.com/jbogard/Respawn) – Resets the test database between integration tests
- [Playwright for .NET](https://playwright.dev/dotnet/) – Drives a real browser in the E2E tests
- [Microsoft.Testing.Extensions.CodeCoverage](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-extensions-code-coverage) – Collects code coverage

## Build and development tools

- [pnpm](https://pnpm.io/) – The Node package manager for the Tailwind CLI (npm is not supported)
- [Docker](https://docs.docker.com/) and [Docker Compose](https://docs.docker.com/compose/) – The local database and the test containers
- [EditorConfig](https://editorconfig.org/) – Formatting and code style rules in `.editorconfig`
- [NuGet Central Package Management](https://learn.microsoft.com/nuget/consume-packages/central-package-management) – All package versions in one `Directory.Packages.props`

## CI and repository automation

- [GitHub Actions](https://docs.github.com/actions) – Build, test, lint, release, and merge workflows
- [CodeQL](https://codeql.github.com/docs/) – Security analysis of the C# code and the workflows
- [Dependabot](https://docs.github.com/code-security/dependabot) – Updates for NuGet, the .NET SDK, npm, and GitHub Actions
- [Codecov](https://docs.codecov.com/) – Coverage reports on pull requests
- [GitHub Copilot code review](https://docs.github.com/copilot/using-github-copilot/code-review/using-copilot-code-review) – Automatic review of each push to a pull request
- [markdownlint-cli2](https://github.com/DavidAnson/markdownlint-cli2) – Markdown linting, in CI and in the git hooks
- [yamllint](https://yamllint.readthedocs.io/) – YAML linting
- [actionlint](https://github.com/rhysd/actionlint) – GitHub Actions workflow linting
- [zizmor](https://docs.zizmor.sh/) – Security linting for GitHub Actions workflows
- [ShellCheck](https://www.shellcheck.net/) – Shell script linting
- [create-pull-request](https://github.com/peter-evans/create-pull-request) – Opens the code metrics pull request when the metrics workflow is run by hand

## Guides

- [Blazor Server and EF Core](https://learn.microsoft.com/aspnet/core/blazor/blazor-ef-core) – Why the app uses `IDbContextFactory` for a short-lived context per query
- [Conventional Commits](https://www.conventionalcommits.org/) – The basis for this repo's commit message format
- [Contributor Covenant](https://www.contributor-covenant.org/) – The code of conduct
