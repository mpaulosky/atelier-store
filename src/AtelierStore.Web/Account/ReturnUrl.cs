using System.Diagnostics.CodeAnalysis;

namespace AtelierStore.Web.Account;

/// <summary>Validates post-login return URLs so sign-in can't redirect off-site (open-redirect guard).</summary>
internal static class ReturnUrl
{
	/// <summary>
	/// True only for root-relative paths. "//host" and "/\host" are scheme-relative to browsers,
	/// so they would redirect off-site and are rejected.
	/// </summary>
	public static bool IsLocal([NotNullWhen(true)] string? url) =>
		url is ['/', ..] && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'));
}
