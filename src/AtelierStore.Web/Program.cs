using Auth0.AspNetCore.Authentication;
using AtelierStore.Web.Components;
using AtelierStore.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// Load a local .env (searching parent directories) into environment variables.
// Real environment variables take precedence over values in the file.
DotNetEnv.Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

string RequiredSetting(string key) =>
    builder.Configuration[key] is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException($"Missing required configuration '{key}'. See .env.example.");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Auth0 (OpenID Connect + cookie session)
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = RequiredSetting("Auth0:Domain");
    options.ClientId = RequiredSetting("Auth0:ClientId");
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(RequiredSetting("ConnectionStrings:Default")));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("postgres");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/account/login", async (HttpContext context, string? returnUrl) =>
{
    var properties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(string.IsNullOrEmpty(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative) ? "/" : returnUrl)
        .Build();
    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, properties);
});

app.MapGet("/account/logout", async (HttpContext context) =>
{
    var properties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();
    await context.SignOutAsync(Auth0Constants.AuthenticationScheme, properties);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapHealthChecks("/health");

app.Run();
