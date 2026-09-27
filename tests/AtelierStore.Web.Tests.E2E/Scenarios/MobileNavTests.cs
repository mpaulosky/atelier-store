using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 7: at a phone viewport, the "Menu" drawer opens and lists the primary navigation.</summary>
public sealed class MobileNavTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	private static readonly string[] PrimaryNavLabels = ["New In", "Women", "Men", "Bags", "Jewelry"];

	protected override Task<IBrowserContext> CreateContextAsync() =>
		Fixture.NewContextAsync(options: new BrowserNewContextOptions { ViewportSize = new ViewportSize { Width = 390, Height = 844 } });

	[Fact]
	public async Task OpeningMenu_ShowsPrimaryNavigationLinks()
	{
		// Arrange
		await Page.GotoAsync("/");
		ILocator drawer = Page.Locator("nav.nav-drawer-panel");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Menu", Exact = true }).ClickAsync();

		// Assert
		foreach (string label in PrimaryNavLabels)
		{
			await Expect(drawer.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = label, Exact = true })).ToBeVisibleAsync();
		}
	}
}
