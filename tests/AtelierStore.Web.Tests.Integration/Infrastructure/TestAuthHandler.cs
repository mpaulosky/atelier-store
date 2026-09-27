using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// Lets a test choose anonymous (the default, no header) or signed-in (send <see cref="SignedInUserHeader"/>)
/// without ever involving Auth0. Registered as the default authenticate scheme by
/// <see cref="AtelierWebApplicationFactory"/>; the "Auth0" and "Cookies" schemes it ships alongside are
/// untouched, so endpoints that explicitly challenge/sign out against those schemes still exercise the real code.
/// </summary>
public sealed class TestAuthHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	public const string SchemeName = "TestAuth";

	/// <summary>Send this request header with a user name to authenticate as that user; omit it to stay anonymous.</summary>
	public const string SignedInUserHeader = "X-Test-Signed-In-User";

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Request.Headers.TryGetValue(SignedInUserHeader, out Microsoft.Extensions.Primitives.StringValues values) ||
			values.Count == 0 ||
			string.IsNullOrEmpty(values[0]))
		{
			return Task.FromResult(AuthenticateResult.NoResult());
		}

		string userName = values[0]!;
		ClaimsIdentity identity = new(
			[new Claim(ClaimTypes.NameIdentifier, userName), new Claim(ClaimTypes.Name, userName)],
			SchemeName);
		AuthenticationTicket ticket = new(new ClaimsPrincipal(identity), SchemeName);
		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}
