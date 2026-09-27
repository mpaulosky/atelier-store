using System.Text.RegularExpressions;
using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 1: the home page's "New in" grid links through to matching product pages.</summary>
public sealed class HomeToProductTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task Home_ShowsEightNewInCards()
	{
		// Arrange
		await Page.GotoAsync("/");

		// Act
		ILocator newInCards = Page.Locator("section[aria-labelledby='new-in-title'] .product-card-link");

		// Assert
		await Expect(newInCards).ToHaveCountAsync(8);
	}

	[Fact]
	public async Task ClickingNewInCard_OpensProductDetailPage_WithMatchingTitleAndPrice()
	{
		// Arrange
		await Page.GotoAsync("/");
		ILocator firstCard = Page.Locator("section[aria-labelledby='new-in-title'] .product-card-link").First;
		// TextContentAsync (not InnerTextAsync) so a CSS text-transform on the card doesn't make this
		// mismatch the product page's differently-styled <h1>: both read the same underlying DOM text.
		string expectedName = (await firstCard.TextContentAsync() ?? string.Empty).Trim();
		string expectedHref = await firstCard.GetAttributeAsync("href") ?? throw new InvalidOperationException("Card link has no href.");
		string expectedPrice = (await Page.Locator("section[aria-labelledby='new-in-title'] .product-card-price").First.InnerTextAsync()).Trim();

		// Act
		await firstCard.ClickAsync();

		// Assert
		await Expect(Page).ToHaveURLAsync(new Regex($"/{Regex.Escape(expectedHref)}$"));
		await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Level = 1 })).ToHaveTextAsync(expectedName);
		await Expect(Page.Locator(".product-detail-info p.lead")).ToContainTextAsync(expectedPrice);
	}
}
