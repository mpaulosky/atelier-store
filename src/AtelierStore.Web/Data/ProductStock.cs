namespace AtelierStore.Web.Data;

/// <summary>On-hand inventory for one product. Kept apart from <see cref="Product"/> so stock writes don't touch catalog rows.</summary>
public class ProductStock
{
	public int ProductId { get; set; }

	public Product Product { get; set; } = null!;

	public int Quantity { get; set; }

	public DateTimeOffset UpdatedAt { get; set; }
}
