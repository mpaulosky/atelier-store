using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// A <see cref="WebApplicationFactory{TEntryPoint}"/> pointed at the shared Postgres container, with Auth0
/// replaced end-to-end by fakes so no test ever calls the real Auth0 tenant:
/// <list type="bullet">
/// <item>a fixed OpenID Connect <see cref="OpenIdConnectConfiguration"/> on the "Auth0" scheme, so the
/// handler never fetches discovery metadata from a real (or fake) network address;</item>
/// <item><see cref="TestAuthHandler"/> registered as the default authenticate scheme, so a test can opt a
/// request into being signed in by sending <see cref="TestAuthHandler.SignedInUserHeader"/>, without a real
/// cookie or Auth0 round-trip.</item>
/// </list>
/// The actual guarantee that the real Auth0 tenant and the developer's database are never touched is
/// <see cref="PostgresContainerFixture"/> setting environment variables before Program.cs runs (see its
/// comments); the settings here additionally pin the same values through configuration, and
/// <c>Configuration_ConnectionString_ResolvesToTestContainer</c> asserts the running app actually resolved
/// the container's connection string.
/// </summary>
public sealed class AtelierWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseSetting("ConnectionStrings:Default", connectionString);
		builder.UseSetting("Auth0:Domain", TestAuth0.FakeDomain);
		builder.UseSetting("Auth0:ClientId", TestAuth0.FakeClientId);

		builder.ConfigureTestServices(services =>
		{
			services
				.AddAuthentication()
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

			services.Configure<AuthenticationOptions>(authOptions =>
			{
				authOptions.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
				authOptions.DefaultScheme = TestAuthHandler.SchemeName;
			});

			services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, oidcOptions =>
			{
				oidcOptions.Configuration = new OpenIdConnectConfiguration
				{
					Issuer = TestAuth0.Issuer,
					AuthorizationEndpoint = TestAuth0.AuthorizationEndpoint,
					TokenEndpoint = TestAuth0.TokenEndpoint,
					EndSessionEndpoint = TestAuth0.EndSessionEndpoint,
					JwksUri = TestAuth0.JwksUri,
				};
			});
		});
	}

	/// <summary>An <see cref="HttpClient"/> that does not auto-follow redirects, so tests can inspect the raw 302 to the fake Auth0 host.</summary>
	public HttpClient CreateClientWithoutRedirects() =>
		CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
}
