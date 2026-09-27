using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 5: an unknown product slug renders the site's not-found page instead of erroring.</summary>
public sealed class ProductNotFoundTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task UnknownSlug_RendersNotFoundPage()
	{
		// Arrange & Act
		await Page.GotoAsync("/products/does-not-exist");

		// Assert
		await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "This page is not available" }))
			.ToBeVisibleAsync();
	}
}
