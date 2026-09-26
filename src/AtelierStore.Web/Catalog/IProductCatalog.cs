namespace AtelierStore.Web.Catalog;

/// <summary>Read-only storefront queries over the catalog, returning the <see cref="Product"/> view record.</summary>
public interface IProductCatalog
{
	/// <summary>The most recently added products, newest first.</summary>
	Task<IReadOnlyList<Product>> GetNewArrivalsAsync(int count, CancellationToken cancellationToken = default);

	/// <summary>Finds a product by its URL slug (case-insensitive), or returns <see langword="null"/> when none matches.</summary>
	Task<Product?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default);

	/// <summary>Other products to suggest alongside <paramref name="slug"/>: same category first, then newest.</summary>
	Task<IReadOnlyList<Product>> GetRelatedAsync(string slug, int count, CancellationToken cancellationToken = default);
}
