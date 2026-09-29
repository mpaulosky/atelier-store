using AtelierStore.Web.Catalog;
using AtelierStore.Web.Components.Catalog;
using Bunit;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class ProductCardTests : BunitContext
{
	[Fact]
	public void ProductCard_SoldOutWithBadge_ShowsSoldOutInsteadOfBadge()
	{
		// Arrange
		Product product = TestProducts.SoldOut(badge: "New");

		// Act
		IRenderedComponent<ProductCard> cut = Render<ProductCard>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.Find(".badge").TextContent.Should().Be("Sold out");
		cut.FindAll(".badge").Should().HaveCount(1);
	}

	[Fact]
	public void ProductCard_InStockWithBadge_ShowsBadge()
	{
		// Arrange
		Product product = TestProducts.InStock(badge: "New");

		// Act
		IRenderedComponent<ProductCard> cut = Render<ProductCard>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.Find(".badge").TextContent.Should().Be("New");
	}

	[Fact]
	public void ProductCard_InStockWithoutBadge_RendersNoBadge()
	{
		// Arrange
		Product product = TestProducts.InStock();

		// Act
		IRenderedComponent<ProductCard> cut = Render<ProductCard>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.FindAll(".badge").Should().BeEmpty();
	}

	[Fact]
	public void ProductCard_Rendered_LinksToProductDetailPage()
	{
		// Arrange
		Product product = TestProducts.InStock(slug: "wool-coat");

		// Act
		IRenderedComponent<ProductCard> cut = Render<ProductCard>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.Find("a.product-card-link").GetAttribute("href").Should().Be("products/wool-coat");
	}

	[Theory]
	[InlineData(null, "H3")]
	[InlineData(2, "H2")]
	public void ProductCard_HeadingLevel_SetsTheNameHeadingTag(int? headingLevel, string expectedTag)
	{
		// Arrange
		Product product = TestProducts.InStock(name: "Wool Coat");

		// Act
		IRenderedComponent<ProductCard> cut = Render<ProductCard>(parameters =>
		{
			parameters.Add(p => p.Product, product);
			if (headingLevel is int level)
			{
				parameters.Add(p => p.HeadingLevel, level);
			}
		});

		// Assert
		AngleSharp.Dom.IElement name = cut.Find(".product-card-name");
		name.TagName.Should().Be(expectedTag);
		name.QuerySelector("a.product-card-link")!.TextContent.Should().Be("Wool Coat");
	}
}
