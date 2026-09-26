---
description: 'Blazor component and application patterns'
applyTo: '**/*.razor, **/*.razor.cs, **/*.razor.css'
---

# Blazor Instructions

Atelier Store is a Blazor Web App on .NET 10 and C# 14.
`CLAUDE.md` at the repository root and `src/AtelierStore.Web/CLAUDE.md` describe the project; if this file and they disagree, they win.

## Structure

- `Components/Pages/` holds routable pages, `Components/Layout/` the site chrome, `Components/Catalog/` the product components,
  and `Components/Shared/` small primitives such as `Icon`.
- `Catalog/` holds the view records the components render (`Product`, `Collection`) and `ProductCatalog`, the storefront queries.
  `Data/` holds the EF Core entities, `AppDbContext` and seed data.
- Keep markup in `.razor` files. Move non-trivial logic into an injected service rather than a large `@code` block.
- Get services through `@inject` or `[Inject]`. Never `new` them up.

## Rendering

- The Interactive Server render mode is registered, but `Routes` has no render mode, so pages are statically rendered.
  Add `@rendermode InteractiveServer` only to a component that needs live interactivity.
- Handle form posts the static way: `EditForm` with `FormName` and `[SupplyParameterFromForm]`, or a plain `<form>` with
  `@formname`, `@onsubmit` and `<AntiforgeryToken />`. Add `Enhance` or `data-enhance` for enhanced navigation.
- Load data in `OnInitializedAsync` or `OnParametersSetAsync`, not in constructors.
- For a route whose record doesn't exist, call `NavigationManager.NotFound()`; the router renders `Pages/NotFound` with a 404.
- Call JavaScript interop only from `OnAfterRenderAsync`, never while prerendering. Guard one-time setup with `if (firstRender)`.
- Implement `IDisposable` or `IAsyncDisposable` to release event subscriptions, timers, and `CancellationTokenSource`s.

## Data

- Components read products only through `ProductCatalog`. Never inject `AppDbContext` into a component.
- `ProductCatalog` creates one short-lived context per query from `IDbContextFactory<AppDbContext>` and projects into view records,
  so components never hold tracked entities.
- Changing the seeded catalog in `Data/CatalogSeedData.cs` means adding an EF Core migration.

## Forms and validation

- Validate form models with data annotations and `<DataAnnotationsValidator />`, and show messages with `<ValidationMessage>`.
- Validate again on the server before acting on a post. Never trust client-side validation alone.

## Security

- Authentication uses Auth0 through `Auth0.AspNetCore.Authentication` with a cookie session. Tokens stay on the server.
- Protect pages with `[Authorize]`. `<AuthorizeView>` only shows or hides UI and isn't a security boundary.
- Login and logout are minimal-API endpoints in `Program.cs`, not pages, because they issue challenges and redirects.

## Styling

- Style with the Tailwind CSS v4 design system in `src/AtelierStore.Web/Styles/app.css`: tokens in `@theme`, primitives such as
  `btn`, `eyebrow`, `product-grid` and `product-card` in `@layer components`. Reuse a primitive before adding utilities.
- Tailwind scans only `Components/`, so classes used elsewhere aren't generated. Avoid inline `style` attributes.
- Use `.razor.css` isolation only when utilities and the design system can't express the style.

## Testing

- There is no test project yet. When one is added, test components with bUnit and services with xUnit v3,
  following `dotnet-project.instructions.md`.
