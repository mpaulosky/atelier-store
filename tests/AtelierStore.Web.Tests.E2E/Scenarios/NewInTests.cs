using System.Text.RegularExpressions;
using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>The home page's "View all" opens the full New in page, whose cards link through to product pages.</summary>
public sealed class NewInTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task ViewAll_OpensNewInPage_LeadingWithTheHomePagesNewInPieces()
	{
		// Arrange
		await Page.GotoAsync("/");
		ILocator homeSection = Page.Locator("section[aria-labelledby='new-in-title']");
		IReadOnlyList<string> homeHrefs = await CardHrefsAsync(homeSection.Locator(".product-card-link"));

		// Act
		await homeSection.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "View all" }).ClickAsync();

		// Assert
		// Both lists come from the same newest-first query, so this holds however large the catalog grows.
		await Expect(Page).ToHaveURLAsync(new Regex("/new-in$"));
		await Expect(Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Level = 1 })).ToHaveTextAsync("New in");
		IReadOnlyList<string> newInHrefs = await CardHrefsAsync(Page.Locator(".product-card-link"));
		homeHrefs.Should().NotBeEmpty();
		newInHrefs.Take(homeHrefs.Count).Should().Equal(homeHrefs);
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

	private static async Task<IReadOnlyList<string>> CardHrefsAsync(ILocator cardLinks)
	{
		await Expect(cardLinks.First).ToBeVisibleAsync();
		List<string> hrefs = [];
		foreach (ILocator link in await cardLinks.AllAsync())
		{
			hrefs.Add(await link.GetAttributeAsync("href") ?? throw new InvalidOperationException("Card link has no href."));
		}

		return hrefs;
	}
}
