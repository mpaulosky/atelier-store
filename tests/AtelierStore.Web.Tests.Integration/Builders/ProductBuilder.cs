using AtelierStore.Web.Data;

namespace AtelierStore.Web.Tests.Integration.Builders;

/// <summary>Builds a minimal <see cref="Product"/> row for a test to insert directly.</summary>
public sealed class ProductBuilder
{
	private string _slug = "test-product";

	private string _name = "Test Product";

	private int _categoryId = 1;

	private decimal _price = 100m;

	private decimal? _wasPrice;

	private string _imageId = "test-image-id";

	private string? _badge;

	private DateTimeOffset _createdAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

	public ProductBuilder WithSlug(string slug)
	{
		_slug = slug;
		return this;
	}

	public ProductBuilder WithName(string name)
	{
		_name = name;
		return this;
	}

	public ProductBuilder InCategory(int categoryId)
	{
		_categoryId = categoryId;
		return this;
	}

	public ProductBuilder WithPrice(decimal price)
	{
		_price = price;
		return this;
	}

	public ProductBuilder WithWasPrice(decimal? wasPrice)
	{
		_wasPrice = wasPrice;
		return this;
	}

	public ProductBuilder WithBadge(string? badge)
	{
		_badge = badge;
		return this;
	}

	public ProductBuilder CreatedAt(DateTimeOffset createdAt)
	{
		_createdAt = createdAt;
		return this;
	}

	public Product Build() => new()
	{
		Slug = _slug,
		Name = _name,
		Description = "A test product.",
		CategoryId = _categoryId,
		Price = _price,
		WasPrice = _wasPrice,
		ImageId = _imageId,
		Badge = _badge,
		CreatedAt = _createdAt,
	};
}
