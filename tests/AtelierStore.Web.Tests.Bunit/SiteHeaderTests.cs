using AtelierStore.Web.Components.Layout;
using Bunit;
using Bunit.TestDoubles;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class SiteHeaderTests : BunitContext
{
	[Fact]
	public void SiteHeader_AnonymousUser_ShowsLogInLink()
	{
		// Arrange
		BunitAuthorizationContext authContext = AddAuthorization();
		authContext.SetNotAuthorized();

		// Act
		IRenderedComponent<SiteHeader> cut = Render<SiteHeader>();

		// Assert
		cut.Find("a[href='account/login'] svg").GetAttribute("aria-label").Should().Be("Log in");
		cut.FindAll("a[href='account/logout']").Should().BeEmpty();
	}

	[Fact]
	public void SiteHeader_SignedInUser_ShowsLogOutLink()
	{
		// Arrange
		BunitAuthorizationContext authContext = AddAuthorization();
		authContext.SetAuthorized("jane@example.com");

		// Act
		IRenderedComponent<SiteHeader> cut = Render<SiteHeader>();

		// Assert
		cut.Find("a[href='account/logout'] svg").GetAttribute("aria-label").Should().Be("Log out");
		cut.FindAll("a[href='account/login']").Should().BeEmpty();
	}

	[Fact]
	public void SiteHeader_MobileMenuToggle_IsANamedButton()
	{
		// Arrange
		BunitAuthorizationContext authContext = AddAuthorization();
		authContext.SetNotAuthorized();

		// Act
		IRenderedComponent<SiteHeader> cut = Render<SiteHeader>();

		// Assert: <summary> has no implicit ARIA role, so the toggle needs an explicit one for its label to be announced (#18).
		AngleSharp.Dom.IElement toggle = cut.Find("details.nav-drawer > summary");
		toggle.GetAttribute("role").Should().Be("button");
		toggle.GetAttribute("aria-label").Should().Be("Menu");
	}
}
