# CLAUDE.md — AtelierStore.Web

Blazor Web App using the Interactive Server render mode. Solution-wide commands and conventions are in the root `CLAUDE.md`.

## Commands

```bash
# Skip the Tailwind step (e.g. no Node/pnpm available)
dotnet build -p:SkipTailwind=true

# Live CSS rebuild during UI work (run in this folder)
pnpm run css:watch
```

## Architecture notes

- **All service wiring lives in `Program.cs`** (top-level statements): Razor components, Auth0, EF Core, health checks, and the minimal-API endpoints `/account/login`, `/account/logout`, and `/health`.
- **Configuration loading**: `DotNetEnv` loads `.env` at startup, searching parent directories, with `NoClobber` so real environment variables win. The local `RequiredSetting` helper throws at startup if `Auth0:Domain`, `Auth0:ClientId`, or `ConnectionStrings:Default` is empty. `appsettings.json` keeps those keys deliberately blank.
- **Auth**: Auth0 OIDC plus a cookie session (`AddAuth0WebAppAuthentication`). Login/logout are plain HTTP endpoints, not Blazor pages, because they must issue challenges and redirects. `login` only accepts relative `returnUrl`s (open-redirect guard). The Blazor side uses `AddCascadingAuthenticationState` and `AuthorizeRouteView` in `Components/Routes.razor`, so pages can use `[Authorize]` and `<AuthorizeView>`. The Auth0 callback URL is `https://localhost:7207/callback`.
- **Tailwind build is part of MSBuild**: the csproj runs `pnpm install --frozen-lockfile` if `node_modules` is missing, then `pnpm run css:build` before every build. The source is `Styles/app.css` (`@source "../Components"` scans Razor files for classes). The output `wwwroot/app.css` is generated, so don't edit it by hand. Classes used outside `Components/` won't be picked up.
- **Error routing**: `/Error` handles exceptions outside Development. Status codes re-execute to `/not-found` (the Router's `NotFoundPage`).
