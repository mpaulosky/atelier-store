using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AtelierStore.Web.Tests.E2E.Infrastructure;

/// <summary>
/// Boots the storefront on a real, in-process Kestrel listener (not <c>TestServer</c>), because Playwright
/// drives it through a real browser. Runs in the <c>Development</c> environment (so static web assets and
/// the Blazor Server script are served), points at the E2E-owned Testcontainers Postgres instance, and
/// swaps Auth0 for <see cref="TestAuthHandler"/> so no test ever reaches the real identity provider.
/// </summary>
/// <remarks>
/// Call <see cref="StartOnKestrelAsync"/> once (before any test uses <see cref="ServerAddress"/>) instead of
/// touching <see cref="WebApplicationFactory{TEntryPoint}.Server"/>: that property assumes <c>TestServer</c>
/// and throws once <see cref="WebApplicationFactory{TEntryPoint}.UseKestrel()"/> has been requested.
/// </remarks>
public sealed class AtelierAppFactory(string connectionString) : WebApplicationFactory<Program>
{
	/// <summary>The base URL of the running Kestrel listener, populated by <see cref="StartOnKestrelAsync"/>.</summary>
	public string ServerAddress => ClientOptions.BaseAddress.ToString();

	/// <summary>Starts the app on a real Kestrel listener bound to a dynamic loopback port.</summary>
	public Task StartOnKestrelAsync()
	{
		UseKestrel(0);
		StartServer();
		return Task.CompletedTask;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment(Environments.Development);

		// Belt-and-suspenders alongside the fixture's process environment variables: this configuration
		// source is added after the app's own (Program.cs runs DotNetEnv before this hook fires), so it
		// wins regardless. The app must never see the developer's real Auth0 tenant or Postgres database.
		builder.ConfigureAppConfiguration((_, configBuilder) => configBuilder.AddInMemoryCollection(
		[
			new KeyValuePair<string, string?>("Auth0:Domain", "e2e-tests.example.com"),
			new KeyValuePair<string, string?>("Auth0:ClientId", "e2e-test-client-id"),
			new KeyValuePair<string, string?>("Auth0:ClientSecret", "e2e-test-client-secret"),
			new KeyValuePair<string, string?>("ConnectionStrings:Default", connectionString),
		]));

		builder.ConfigureServices(services => services
			.AddAuthentication(options =>
			{
				// AddAuth0WebAppAuthentication (Program.cs) explicitly sets DefaultAuthenticateScheme to its
				// own cookie scheme, which wins over DefaultScheme in the authentication middleware. Every one
				// of these needs to point at the fake scheme, or requests still authenticate against Auth0's
				// (inert, in this host) cookie handler and every visitor reads as anonymous.
				options.DefaultScheme = TestAuthHandler.SchemeName;
				options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
				options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
				options.DefaultSignInScheme = TestAuthHandler.SchemeName;
			})
			.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, configureOptions: null));
	}
}
