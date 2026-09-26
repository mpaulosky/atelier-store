namespace AtelierStore.Web.Data;

public class Product
{
	public int Id { get; set; }

	/// <summary>Lowercase URL key, unique across products.</summary>
	public required string Slug { get; set; }

	public required string Name { get; set; }

	public required string Description { get; set; }

	public int CategoryId { get; set; }

	public Category Category { get; set; } = null!;

	public decimal Price { get; set; }

	/// <summary>The original price when the product is on sale; always greater than <see cref="Price"/>.</summary>
	public decimal? WasPrice { get; set; }

	/// <summary>Unsplash photo ID.</summary>
	public required string ImageId { get; set; }

	public string? Badge { get; set; }

	public DateTimeOffset CreatedAt { get; set; }

	public ProductStock Stock { get; set; } = null!;
}
