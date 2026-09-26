using AtelierStore.Web.Catalog;

namespace AtelierStore.Web.Tests.Bunit;

/// <summary>Builds <see cref="Product"/> instances for component tests, with sensible defaults for the fields a given test does not care about.</summary>
internal static class TestProducts
{
	public static Product InStock(
		string slug = "wool-coat",
		string name = "Wool Coat",
		string category = "Outerwear",
		decimal price = 480m,
		string? badge = null,
		decimal? wasPrice = null) =>
		new(slug, name, category, price, "1483985988355-763728e1935b", "A tailored wool coat.", Product.LowStockThreshold + 1, badge, wasPrice);

	public static Product LowStock(int stock = 2, string slug = "wool-coat", string name = "Wool Coat") =>
		new(slug, name, "Outerwear", 480m, "1483985988355-763728e1935b", "A tailored wool coat.", stock);

	public static Product SoldOut(string slug = "wool-coat", string name = "Wool Coat", string? badge = null) =>
		new(slug, name, "Outerwear", 480m, "1483985988355-763728e1935b", "A tailored wool coat.", 0, badge);
}
