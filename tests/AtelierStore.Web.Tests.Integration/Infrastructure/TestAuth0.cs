namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// The fake Auth0 tenant every test host points at, so no test ever talks to the real Auth0 tenant.
/// </summary>
internal static class TestAuth0
{
	public const string FakeDomain = "fake-auth0.test";

	public const string FakeClientId = "test-client-id";

	public const string FakeClientSecret = "test-client-secret";

	public static string AuthorizationEndpoint => $"https://{FakeDomain}/authorize";

	public static string TokenEndpoint => $"https://{FakeDomain}/oauth/token";

	public static string EndSessionEndpoint => $"https://{FakeDomain}/v2/logout";

	public static string JwksUri => $"https://{FakeDomain}/.well-known/jwks.json";

	public static string Issuer => $"https://{FakeDomain}/";
}
