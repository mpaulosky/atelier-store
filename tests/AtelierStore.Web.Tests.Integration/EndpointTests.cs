using System.Net;
using Auth0.AspNetCore.Authentication;
using AtelierStore.Web.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace AtelierStore.Web.Tests.Integration;

/// <summary>
/// <c>/health</c>, <c>/account/login</c> and <c>/account/logout</c> against a full test host
/// (<see cref="AtelierWebApplicationFactory"/>), with Auth0 replaced by fakes so no request ever reaches
/// the real Auth0 tenant. Every client disables auto-redirect, so 302s to the fake Auth0 host are asserted
/// directly instead of the <see cref="HttpClient"/> trying to follow them onto a host that doesn't exist.
/// </summary>
[Collection(DatabaseCollection.Name)]
public sealed class EndpointTests(PostgresContainerFixture fixture) : IDisposable
{
	private readonly AtelierWebApplicationFactory _factory = new(fixture.ConnectionString);

	public void Dispose() => _factory.Dispose();

	[Fact]
	public async Task Health_ReturnsHealthy()
	{
		// Arrange
		HttpClient client = _factory.CreateClient();

		// Act
		HttpResponseMessage response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		body.Should().Be("Healthy");
	}

	[Fact]
	public async Task Configuration_ConnectionString_ResolvesToTheTestContainerNotTheDeveloperDatabase()
	{
		// Arrange
		Npgsql.NpgsqlConnectionStringBuilder expected = new(fixture.ConnectionString);

		// Act
		IConfiguration configuration = _factory.Services.GetRequiredService<IConfiguration>();
		Npgsql.NpgsqlConnectionStringBuilder actual = new(configuration["ConnectionStrings:Default"]!);

		// Assert
		actual.Host.Should().Be(expected.Host);
		actual.Port.Should().Be(expected.Port);
		actual.Database.Should().Be(expected.Database);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("some-signed-in-user")]
	public async Task Login_WithALocalReturnUrl_ChallengesAuth0AndPreservesIt(string? signedInAs)
	{
		// Arrange
		HttpClient client = _factory.CreateClientWithoutRedirects();
		using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/account/login?returnUrl=/products/silk-jogger", UriKind.Relative));
		if (signedInAs is not null)
		{
			request.Headers.Add(TestAuthHandler.SignedInUserHeader, signedInAs);
		}

		// Act
		HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Found);
		response.Headers.Location.Should().NotBeNull();
		response.Headers.Location!.GetLeftPart(UriPartial.Path).Should().Be(TestAuth0.AuthorizationEndpoint);
		AuthenticationProperties properties = UnprotectState(response.Headers.Location!);
		properties.RedirectUri.Should().Be("/products/silk-jogger");
	}

	[Theory]
	[InlineData("//evil.com")]
	[InlineData("/\\evil.com")]
	[InlineData("https://evil.com")]
	[InlineData(null)]
	public async Task Login_WithAnOffSiteOrMissingReturnUrl_RewritesTheRedirectToRoot(string? returnUrl)
	{
		// Arrange
		HttpClient client = _factory.CreateClientWithoutRedirects();
		string path = returnUrl is null ? "/account/login" : $"/account/login?returnUrl={Uri.EscapeDataString(returnUrl)}";

		// Act
		HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative), TestContext.Current.CancellationToken);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Found);
		AuthenticationProperties properties = UnprotectState(response.Headers.Location!);
		properties.RedirectUri.Should().Be("/");
	}

	[Theory]
	[InlineData(null)]
	[InlineData("some-signed-in-user")]
	public async Task Logout_SignsOut_RedirectsTowardsAuth0LogoutWithoutCallingIt(string? signedInAs)
	{
		// Arrange
		HttpClient client = _factory.CreateClientWithoutRedirects();
		using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/account/logout", UriKind.Relative));
		if (signedInAs is not null)
		{
			request.Headers.Add(TestAuthHandler.SignedInUserHeader, signedInAs);
		}

		// Act
		HttpResponseMessage response = await client.SendAsync(request, TestContext.Current.CancellationToken);

		// Assert: a redirect towards the fake Auth0 host's logout endpoint, never the real tenant.
		((int)response.StatusCode).Should().BeInRange(300, 399);
		response.Headers.Location.Should().NotBeNull();
		response.Headers.Location!.Host.Should().Be(TestAuth0.FakeDomain);
	}

	/// <summary>
	/// Unprotects the OIDC "state" query parameter using the exact same <c>StateDataFormat</c> the running
	/// app's "Auth0" scheme is configured with, so the assertion reads precisely what a real client (or
	/// Auth0 itself) would decode from the redirect the server actually sent - the most direct way to
	/// observe that the sanitized return URL survived into the challenge, without a production-code hook.
	/// </summary>
	private AuthenticationProperties UnprotectState(Uri redirectLocation)
	{
		Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(redirectLocation.Query);
		string state = query["state"].ToString();
		OpenIdConnectOptions options = _factory.Services
			.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>()
			.Get(Auth0Constants.AuthenticationScheme);
		return options.StateDataFormat.Unprotect(state) ?? throw new InvalidOperationException("Could not unprotect the OIDC state.");
	}
}
