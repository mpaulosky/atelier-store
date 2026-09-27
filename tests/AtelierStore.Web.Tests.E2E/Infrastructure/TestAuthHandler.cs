using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AtelierStore.Web.Tests.E2E.Infrastructure;

/// <summary>
/// Fake authentication scheme for the E2E host: never calls Auth0. A request authenticates only when
/// it carries the <see cref="CookieName"/> cookie set to <see cref="CookieValue"/> (a browser sets it
/// via <c>IBrowserContext.AddCookiesAsync</c>); every other request is anonymous.
/// </summary>
public sealed class TestAuthHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	public const string SchemeName = "E2ETest";

	public const string CookieName = "e2e-auth";

	public const string CookieValue = "signed-in";

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (Request.Cookies[CookieName] != CookieValue)
		{
			return Task.FromResult(AuthenticateResult.NoResult());
		}

		Claim[] claims = [new(ClaimTypes.Name, "Atelier Test Client")];
		ClaimsIdentity identity = new(claims, SchemeName);
		ClaimsPrincipal principal = new(identity);
		AuthenticationTicket ticket = new(principal, SchemeName);
		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}
