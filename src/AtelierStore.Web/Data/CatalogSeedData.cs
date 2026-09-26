namespace AtelierStore.Web.Data;

/// <summary>
/// The starter catalog, inserted by the InitialCatalog migration through <c>HasData</c>.
/// IDs and timestamps are fixed so the migration is deterministic; <see cref="Product.CreatedAt"/>
/// descends in list order so "New in" shows the products in this order.
/// </summary>
internal static class CatalogSeedData
{
	private static readonly DateTimeOffset SeededAt = new(2026, 9, 1, 0, 0, 0, TimeSpan.Zero);

	public static Category[] Categories { get; } =
	[
		new() { Id = 1, Slug = "outerwear", Name = "Outerwear" },
		new() { Id = 2, Slug = "knitwear", Name = "Knitwear" },
		new() { Id = 3, Slug = "trousers", Name = "Trousers" },
		new() { Id = 4, Slug = "handbags", Name = "Handbags" },
		new() { Id = 5, Slug = "shoes", Name = "Shoes" },
		new() { Id = 6, Slug = "eyewear", Name = "Eyewear" },
		new() { Id = 7, Slug = "jewelry", Name = "Jewelry" },
	];

	public static Product[] Products { get; } =
	[
		new()
		{
			Id = 1, Slug = "leather-biker-jacket", Name = "Leather Biker Jacket", CategoryId = 1, Price = 2400m,
			ImageId = "1551028719-00167b16eac5", Badge = "New", CreatedAt = SeededAt.AddDays(8),
			Description = "Cut from supple lambskin with a slightly cropped body, asymmetric zip and quilted shoulders. Lined in cupro, so it layers easily over knitwear.",
		},
		new()
		{
			Id = 2, Slug = "hand-knit-poncho", Name = "Hand-Knit Fringe Poncho", CategoryId = 2, Price = 980m,
			ImageId = "1434389677669-e08b4cac3105", CreatedAt = SeededAt.AddDays(7),
			Description = "Knitted by hand in a soft wool and alpaca blend, finished with a long fringed hem. Drapes loosely from the shoulders and wears over coats or alone.",
		},
		new()
		{
			Id = 3, Slug = "silk-jogger", Name = "Silk Jogger Trouser", CategoryId = 3, Price = 540m, WasPrice = 720m,
			ImageId = "1594633312681-425c7b97ccd1", CreatedAt = SeededAt.AddDays(6),
			Description = "Washed silk twill with a drawstring waist and cuffed ankles. Relaxed through the leg, with deep side pockets and a fluid, matte finish.",
		},
		new()
		{
			Id = 4, Slug = "top-handle-bag", Name = "Structured Top-Handle Bag", CategoryId = 4, Price = 2950m,
			ImageId = "1584917865442-de89df76afd3", Badge = "Exclusive", CreatedAt = SeededAt.AddDays(5),
			Description = "A structured calfskin bag with a rolled top handle, detachable shoulder strap and palladium hardware. The interior is lined in suede, with one zip pocket.",
		},
		new()
		{
			Id = 5, Slug = "botanical-tote", Name = "Botanical Print Tote", CategoryId = 4, Price = 1850m,
			ImageId = "1591561954557-26941169b49e", CreatedAt = SeededAt.AddDays(4),
			Description = "An open canvas tote printed with an archive botanical motif and trimmed in vegetable-tanned leather. Roomy enough for a laptop and a day's essentials.",
		},
		new()
		{
			Id = 6, Slug = "panelled-runner-sneaker", Name = "Panelled Runner Sneaker", CategoryId = 5, Price = 790m,
			ImageId = "1560769629-975ec94e6a86", Badge = "New", CreatedAt = SeededAt.AddDays(3),
			Description = "A retro runner built from suede and technical mesh panels on a lightweight rubber sole. Made in Italy and finished with a padded collar.",
		},
		new()
		{
			Id = 7, Slug = "round-metal-sunglasses", Name = "Round Metal Sunglasses", CategoryId = 6, Price = 390m,
			ImageId = "1511499767150-a48a237f0083", CreatedAt = SeededAt.AddDays(2),
			Description = "Fine round frames in brushed gold-tone metal with adjustable nose pads and tinted lenses offering full UV protection. Comes with a leather case.",
		},
		new()
		{
			Id = 8, Slug = "pearl-collar-necklace", Name = "Pearl Collar Necklace", CategoryId = 7, Price = 1450m,
			ImageId = "1515562141207-7a88fb7ce338", CreatedAt = SeededAt.AddDays(1),
			Description = "Freshwater pearls, hand-knotted on silk and closed with an engraved gold vermeil clasp. Sits close at the base of the neck.",
		},
	];

	public static ProductStock[] Stock { get; } =
	[
		new() { ProductId = 1, Quantity = 6, UpdatedAt = SeededAt },
		new() { ProductId = 2, Quantity = 2, UpdatedAt = SeededAt },
		new() { ProductId = 3, Quantity = 9, UpdatedAt = SeededAt },
		new() { ProductId = 4, Quantity = 1, UpdatedAt = SeededAt },
		new() { ProductId = 5, Quantity = 0, UpdatedAt = SeededAt },
		new() { ProductId = 6, Quantity = 12, UpdatedAt = SeededAt },
		new() { ProductId = 7, Quantity = 7, UpdatedAt = SeededAt },
		new() { ProductId = 8, Quantity = 3, UpdatedAt = SeededAt },
	];
}
