using AtelierStore.Web.Catalog;
using AtelierStore.Web.Components.Catalog;
using AtelierStore.Web.Components.Pages;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace AtelierStore.Web.Tests.Bunit;

public sealed class HomeTests : BunitContext
{
	[Fact]
	public void Home_Rendered_ShowsOneCardPerNewArrival()
	{
		// Arrange
		IReadOnlyList<Product> newArrivals =
		[
			TestProducts.InStock(slug: "wool-coat", name: "Wool Coat"),
			TestProducts.InStock(slug: "silk-scarf", name: "Silk Scarf"),
			TestProducts.SoldOut(slug: "leather-bag", name: "Leather Bag"),
		];
		IProductCatalog catalog = Substitute.For<IProductCatalog>();
		catalog.GetNewArrivalsAsync(8, Arg.Any<CancellationToken>()).Returns(newArrivals);
		Services.AddSingleton(catalog);

		// Act
		IRenderedComponent<Home> cut = Render<Home>();

		// Assert
		cut.FindComponents<ProductCard>().Should().HaveCount(newArrivals.Count);
	}
}
