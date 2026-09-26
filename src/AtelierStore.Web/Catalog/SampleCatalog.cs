namespace AtelierStore.Web.Catalog;

public sealed record Product(string Slug, string Name, string Category, decimal Price, string ImageId, string? Badge = null, decimal? WasPrice = null);

public sealed record Collection(string Slug, string Title, string Kicker, string ImageId);

/// <summary>
/// Hard-coded storefront content used until the catalog is backed by the database.
/// Image IDs are Unsplash photo IDs; build URLs with <see cref="UnsplashImage"/>.
/// </summary>
public static class SampleCatalog
{
	public static IReadOnlyList<Collection> FeaturedCollections { get; } =
	[
		new("women", "Women", "Autumn ready-to-wear", "1483985988355-763728e1935b"),
		new("men", "Men", "Tailoring and outerwear", "1507679799987-c73779587ccf"),
		new("shoes", "Shoes", "Heels, loafers, sneakers", "1543163521-1bf539c55dd2"),
		new("jewelry", "Jewelry", "Pearls and crystal", "1535632066927-ab7c9ab60908"),
	];

	public static IReadOnlyList<Product> NewArrivals { get; } =
	[
		new("leather-biker-jacket", "Leather Biker Jacket", "Outerwear", 2400m, "1551028719-00167b16eac5", "New"),
		new("hand-knit-poncho", "Hand-Knit Fringe Poncho", "Knitwear", 980m, "1434389677669-e08b4cac3105"),
		new("silk-jogger", "Silk Jogger Trouser", "Trousers", 540m, "1594633312681-425c7b97ccd1", WasPrice: 720m),
		new("top-handle-bag", "Structured Top-Handle Bag", "Handbags", 2950m, "1584917865442-de89df76afd3", "Exclusive"),
		new("botanical-tote", "Botanical Print Tote", "Handbags", 1850m, "1591561954557-26941169b49e"),
		new("panelled-runner-sneaker", "Panelled Runner Sneaker", "Shoes", 790m, "1560769629-975ec94e6a86", "New"),
		new("round-metal-sunglasses", "Round Metal Sunglasses", "Eyewear", 390m, "1511499767150-a48a237f0083"),
		new("pearl-collar-necklace", "Pearl Collar Necklace", "Jewelry", 1450m, "1515562141207-7a88fb7ce338"),
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
