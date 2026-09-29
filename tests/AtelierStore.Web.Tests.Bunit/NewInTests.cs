using AngleSharp.Dom;
using AtelierStore.Web.Catalog;
using AtelierStore.Web.Components.Catalog;
using AtelierStore.Web.Components.Pages;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class NewInTests : BunitContext
{
	private readonly IProductCatalog _catalog = Substitute.For<IProductCatalog>();

	public NewInTests() => Services.AddSingleton(_catalog);

	[Fact]
	public void NewIn_Rendered_ShowsOneCardPerNewArrivalInCatalogOrder()
	{
		// Arrange
		IReadOnlyList<Product> newArrivals =
		[
			TestProducts.InStock(slug: "wool-coat", name: "Wool Coat"),
			TestProducts.SoldOut(slug: "leather-bag", name: "Leather Bag"),
			TestProducts.InStock(slug: "silk-scarf", name: "Silk Scarf"),
		];
		_catalog.GetNewArrivalsAsync(24, Arg.Any<CancellationToken>()).Returns(newArrivals);

		// Act
		IRenderedComponent<NewIn> cut = Render<NewIn>();

		// Assert
		cut.FindComponents<ProductCard>().Select(card => card.Instance.Product.Slug)
			.Should().Equal("wool-coat", "leather-bag", "silk-scarf");
		cut.Find(".section-header .caption").TextContent.Should().Be("3 pieces");
		cut.FindAll(".lead").Should().BeEmpty();
		cut.FindAll(".product-card-name").Should().OnlyContain(name => name.TagName == "H2");
	}

	[Fact]
	public void NewIn_WithOneArrival_UsesTheSingularCount()
	{
		// Arrange
		_catalog.GetNewArrivalsAsync(24, Arg.Any<CancellationToken>()).Returns([TestProducts.InStock()]);

		// Act
		IRenderedComponent<NewIn> cut = Render<NewIn>();

		// Assert
		cut.Find(".section-header .caption").TextContent.Should().Be("1 piece");
	}

	[Fact]
	public void NewIn_WithNoArrivals_ShowsTheEmptyStateInsteadOfTheGrid()
	{
		// Arrange
		_catalog.GetNewArrivalsAsync(24, Arg.Any<CancellationToken>()).Returns([]);

		// Act
		IRenderedComponent<NewIn> cut = Render<NewIn>();

		// Assert
		cut.FindAll(".product-grid").Should().BeEmpty();
		cut.FindAll(".section-header .caption").Should().BeEmpty();
		cut.Find(".lead").TextContent.Should().Be("Nothing new just yet. Check back soon.");
		cut.Find("a.link-cta").GetAttribute("href").Should().BeEmpty();
	}

	[Fact]
	public void NewIn_Rendered_HasAPageHeadingAndABreadcrumbBackHome()
	{
		// Arrange
		_catalog.GetNewArrivalsAsync(24, Arg.Any<CancellationToken>()).Returns([]);

		// Act
		IRenderedComponent<NewIn> cut = Render<NewIn>();

		// Assert
		cut.Find("h1").TextContent.Should().Be("New in");
		IReadOnlyList<IElement> crumbs = cut.FindAll("nav.breadcrumb li");
		crumbs.Should().HaveCount(2);
		crumbs[0].QuerySelector("a")!.GetAttribute("href").Should().BeEmpty();
		crumbs[1].QuerySelector("[aria-current='page']")!.TextContent.Should().Be("New in");
	}
}
