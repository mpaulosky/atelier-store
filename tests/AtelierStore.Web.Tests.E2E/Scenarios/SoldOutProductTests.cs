using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 3: a sold-out product disables the purchase action and explains it may return.</summary>
public sealed class SoldOutProductTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task SoldOutProduct_ShowsDisabledButtonAndReturnNotice()
	{
		// Arrange
		await Page.GotoAsync($"/products/{E2EFixture.SoldOutSlug}");

		// Act
		ILocator button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Sold out" });

		// Assert
		await Expect(button).ToBeDisabledAsync();
		await Expect(Page.GetByText("This piece may return")).ToBeVisibleAsync();
	}
}
