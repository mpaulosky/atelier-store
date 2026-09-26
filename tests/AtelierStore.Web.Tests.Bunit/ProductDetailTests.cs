using AngleSharp.Dom;
using AtelierStore.Web.Catalog;
using AtelierStore.Web.Components.Catalog;
using AtelierStore.Web.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class ProductDetailTests : BunitContext
{
	private readonly IProductCatalog catalog = Substitute.For<IProductCatalog>();

	public ProductDetailTests() => Services.AddSingleton(catalog);

	[Fact]
	public void ProductDetail_InStock_ShowsInStockLabel()
	{
		// Arrange
		Product product = TestProducts.InStock();
		ArrangeCatalog(product);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.Find(".stock-status").TextContent.Should().Be("In stock");
	}

	[Fact]
	public void ProductDetail_LowStock_ShowsRemainingUnitCount()
	{
		// Arrange
		Product product = TestProducts.LowStock(stock: 2);
		ArrangeCatalog(product);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.Find(".stock-status").TextContent.Should().Be("Only 2 left");
	}

	[Fact]
	public void ProductDetail_SoldOut_ShowsSoldOutLabelAndDisabledButtonAndReturnNotice()
	{
		// Arrange
		Product product = TestProducts.SoldOut();
		ArrangeCatalog(product);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.Find(".stock-status").TextContent.Should().Be("Sold out");
		IElement button = cut.Find("button[type=submit]");
		button.TextContent.Should().Be("Sold out");
		button.HasAttribute("disabled").Should().BeTrue();
		cut.Markup.Should().Contain("This piece may return.");
	}

	[Fact]
	public void ProductDetail_SoldOutWithBadge_RendersNoBadge()
	{
		// Arrange: sold out is intentionally badge-less on the detail page (CONTEXT.md).
		Product product = TestProducts.SoldOut(badge: "New");
		ArrangeCatalog(product);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.FindAll(".badge").Should().BeEmpty();
	}

	[Fact]
	public void ProductDetail_InStockWithBadge_RendersBadge()
	{
		// Arrange
		Product product = TestProducts.InStock(badge: "New");
		ArrangeCatalog(product);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.Find(".badge").TextContent.Should().Be("New");
	}

	[Fact]
	public void ProductDetail_UnknownSlug_RaisesNotFound()
	{
		// Arrange
		catalog.FindBySlugAsync("missing", Arg.Any<CancellationToken>()).Returns((Product?)null);
		NavigationManager navigation = Services.GetRequiredService<NavigationManager>();
		bool notFoundRaised = false;
		navigation.OnNotFound += (_, _) => notFoundRaised = true;

		// Act
		RenderDetail("missing");

		// Assert
		notFoundRaised.Should().BeTrue();
	}

	[Fact]
	public void ProductDetail_Rendered_ShowsRelatedProductsAsCards()
	{
		// Arrange
		Product product = TestProducts.InStock(slug: "wool-coat");
		Product[] related =
		[
			TestProducts.InStock(slug: "silk-scarf", name: "Silk Scarf"),
			TestProducts.InStock(slug: "leather-bag", name: "Leather Bag"),
		];
		catalog.FindBySlugAsync(product.Slug, Arg.Any<CancellationToken>()).Returns(product);
		catalog.GetRelatedAsync(product.Slug, 4, Arg.Any<CancellationToken>()).Returns(related);

		// Act
		IRenderedComponent<ProductDetail> cut = RenderDetail(product.Slug);

		// Assert
		cut.FindComponents<ProductCard>().Should().HaveCount(related.Length);
		catalog.Received(1).GetRelatedAsync(product.Slug, 4, Arg.Any<CancellationToken>());
	}

	private void ArrangeCatalog(Product product)
	{
		catalog.FindBySlugAsync(product.Slug, Arg.Any<CancellationToken>()).Returns(product);
		catalog.GetRelatedAsync(product.Slug, 4, Arg.Any<CancellationToken>()).Returns((IReadOnlyList<Product>)[]);
	}

	private IRenderedComponent<ProductDetail> RenderDetail(string slug) =>
		Render<ProductDetail>(parameters => parameters.Add(p => p.Slug, slug));
}
