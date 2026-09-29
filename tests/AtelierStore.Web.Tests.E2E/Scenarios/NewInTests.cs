using System.Text.RegularExpressions;
using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>The home page's "View all" opens the full New in page, whose cards link through to product pages.</summary>
public sealed class NewInTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task ViewAll_OpensNewInPage_ListingSoldOutPiecesInPlace()
	{
		// Arrange
		await Page.GotoAsync("/");

		// Act
		await Page.Locator("section[aria-labelledby='new-in-title']").GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "View all" }).ClickAsync();

		// Assert
		await Expect(Page).ToHaveURLAsync(new Regex("/new-in$"));
		await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Level = 1 })).ToHaveTextAsync("New in");
		await Expect(Page.Locator(".product-card-link", new PageLocatorOptions { HasText = E2EFixture.InStockName })).ToHaveCountAsync(1);
		ILocator soldOutCard = Page.Locator(".product-card", new PageLocatorOptions { HasText = E2EFixture.SoldOutName });
		await Expect(soldOutCard.Locator(".badge")).ToHaveTextAsync("Sold out");
	}

	[Fact]
	public async Task ClickingNewInCard_OpensItsProductDetailPage()
	{
		// Arrange
		await Page.GotoAsync("/new-in");
		ILocator firstCard = Page.Locator(".product-card-link").First;
		string expectedName = (await firstCard.TextContentAsync() ?? string.Empty).Trim();
		string expectedHref = await firstCard.GetAttributeAsync("href") ?? throw new InvalidOperationException("Card link has no href.");

		// Act
		await firstCard.ClickAsync();

		// Assert
		await Expect(Page).ToHaveURLAsync(new Regex($"/{Regex.Escape(expectedHref)}$"));
		await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Level = 1 })).ToHaveTextAsync(expectedName);
	}
}
