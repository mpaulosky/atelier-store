using AtelierStore.Web.Catalog;
using AtelierStore.Web.Components.Catalog;
using Bunit;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class ProductPriceTests : BunitContext
{
	[Fact]
	public void ProductPrice_RegularPrice_RendersFormattedAmount()
	{
		// Arrange
		Product product = TestProducts.InStock(price: 480m);

		// Act
		IRenderedComponent<ProductPrice> cut = Render<ProductPrice>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.Markup.Should().Contain("$480");
		cut.Markup.Should().NotContain("price-was");
	}

	[Fact]
	public void ProductPrice_OnSale_RendersSalePriceAndStruckThroughWasPrice()
	{
		// Arrange
		Product product = TestProducts.InStock(price: 320m, wasPrice: 400m);

		// Act
		IRenderedComponent<ProductPrice> cut = Render<ProductPrice>(parameters => parameters
			.Add(p => p.Product, product));

		// Assert
		cut.Find(".price-sale").TextContent.Should().Be("$320");
		cut.Find("s.price-was").TextContent.Should().Be("$400");
		cut.Markup.Should().Contain("Sale price");
		cut.Markup.Should().Contain(", was");
	}
}
