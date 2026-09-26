namespace AtelierStore.Web.Catalog;

public sealed record Product(
	string Slug,
	string Name,
	string Category,
	decimal Price,
	string ImageId,
	string Description,
	int Stock,
	string? Badge = null,
	decimal? WasPrice = null)
{
	/// <summary>At or below this many units the storefront shows a "low stock" notice.</summary>
	public const int LowStockThreshold = 3;

	public StockState StockState => Stock switch
	{
		<= 0 => StockState.SoldOut,
		<= LowStockThreshold => StockState.LowStock,
		_ => StockState.InStock,
	};
}

public enum StockState
{
	InStock,
	LowStock,
	SoldOut,
}

public sealed record Collection(string Slug, string Title, string Kicker, string ImageId);

/// <summary>
/// Hard-coded editorial content. Products live in the database; read them through <see cref="ProductCatalog"/>.
/// Image IDs are Unsplash photo IDs; build URLs with <see cref="UnsplashImage"/>.
/// </summary>
public static class EditorialContent
{
	public static IReadOnlyList<Collection> FeaturedCollections { get; } =
	[
		new("women", "Women", "Autumn ready-to-wear", "1483985988355-763728e1935b"),
		new("men", "Men", "Tailoring and outerwear", "1507679799987-c73779587ccf"),
		new("shoes", "Shoes", "Heels, loafers, sneakers", "1543163521-1bf539c55dd2"),
		new("jewelry", "Jewelry", "Pearls and crystal", "1535632066927-ab7c9ab60908"),
	];
}

public static class UnsplashImage
{
	/// <summary>Builds a resized, format-negotiated Unsplash URL.</summary>
	public static string Url(string imageId, int width) =>
		$"https://images.unsplash.com/photo-{imageId}?auto=format&fit=crop&q=80&w={width}";

	/// <summary>Builds a <c>srcset</c> value covering the given widths.</summary>
	public static string SrcSet(string imageId, params int[] widths) =>
		string.Join(", ", widths.Select(width => $"{Url(imageId, width)} {width}w"));
}
