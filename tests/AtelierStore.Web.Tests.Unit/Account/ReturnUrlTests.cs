using AtelierStore.Web.Account;

namespace AtelierStore.Web.Tests.Unit.Account;

public class ReturnUrlTests
{

	[Theory]
	[InlineData(null, false)]
	[InlineData("", false)]
	[InlineData("/", true)]
	[InlineData("/products/x", true)]
	// Scheme-relative: browsers resolve "//host" against the current scheme and redirect off-site.
	[InlineData("//evil.com", false)]
	// Scheme-relative via a backslash, which some browsers normalize to a forward slash.
	[InlineData("/\\evil.com", false)]
	[InlineData("https://evil.com", false)]
	// No leading slash at all: not root-relative.
	[InlineData("products/x", false)]
	// Percent-encoded slashes are not decoded here, so this reads as a single root-relative
	// segment and passes. It is not exploitable as an open redirect through this guard alone,
	// because nothing downstream decodes the path before using it as a redirect target.
	[InlineData("/%2F%2Fevil.com", true)]
	// Three leading slashes still trip the scheme-relative check on the second character.
	[InlineData("///evil.com", false)]
	// A leading backslash alone is not a leading slash, so it's rejected outright.
	[InlineData("\\evil.com", false)]
	public void IsLocal_GivenUrl_ReturnsWhetherItIsRootRelative(string? url, bool expected)
	{

		// Arrange

		// Act
		bool actual = ReturnUrl.IsLocal(url);

		// Assert
		actual.Should().Be(expected);

	}

}
