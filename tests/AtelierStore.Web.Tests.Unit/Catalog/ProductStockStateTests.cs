using AtelierStore.Web.Catalog;

namespace AtelierStore.Web.Tests.Unit.Catalog;

public class ProductStockStateTests
{

	private static Product CreateProduct(int stock) =>
		new(
			Slug: "test-product",
			Name: "Test Product",
			Category: "Outerwear",
			Price: 100m,
			ImageId: "abc123",
			Description: "A product for tests.",
			Stock: stock);

	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	public void StockState_StockAtOrBelowZero_IsSoldOut(int stock)
	{

		// Arrange
		Product product = CreateProduct(stock);

		// Act
		StockState actual = product.StockState;

		// Assert
		actual.Should().Be(StockState.SoldOut);

	}

	[Theory]
	[InlineData(1)]
	[InlineData(3)]
	public void StockState_StockAboveZeroUpToThreshold_IsLowStock(int stock)
	{

		// Arrange
		Product product = CreateProduct(stock);

		// Act
		StockState actual = product.StockState;

		// Assert
		actual.Should().Be(StockState.LowStock);

	}

	[Fact]
	public void StockState_StockAboveThreshold_IsInStock()
	{

		// Arrange
		Product product = CreateProduct(Product.LowStockThreshold + 1);

		// Act
		StockState actual = product.StockState;

		// Assert
		actual.Should().Be(StockState.InStock);

	}

}
