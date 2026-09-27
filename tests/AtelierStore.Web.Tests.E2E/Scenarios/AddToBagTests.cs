using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 2: adding an in-stock product to the bag through the enhanced, antiforgery-protected form.</summary>
public sealed class AddToBagTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task AddingInStockProductToBag_ShowsAddedConfirmation()
	{
		// Arrange
		await Page.GotoAsync($"/products/{E2EFixture.InStockSlug}");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to bag" }).ClickAsync();

		// Assert
		await Expect(Page.GetByRole(AriaRole.Status)).ToHaveTextAsync("Added to your bag.");
	}
}
