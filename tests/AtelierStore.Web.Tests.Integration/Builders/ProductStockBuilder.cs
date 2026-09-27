using AtelierStore.Web.Data;

namespace AtelierStore.Web.Tests.Integration.Builders;

/// <summary>Builds a minimal <see cref="ProductStock"/> row for a test to insert directly.</summary>
public sealed class ProductStockBuilder
{
	private int _productId;

	private int _quantity = 10;

	private DateTimeOffset _updatedAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

	public ProductStockBuilder ForProduct(int productId)
	{
		_productId = productId;
		return this;
	}

	public ProductStockBuilder WithQuantity(int quantity)
	{
		_quantity = quantity;
		return this;
	}

	public ProductStock Build() => new() { ProductId = _productId, Quantity = _quantity, UpdatedAt = _updatedAt };
}
