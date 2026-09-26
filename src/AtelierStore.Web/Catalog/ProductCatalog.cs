using System.Linq.Expressions;
using AtelierStore.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AtelierStore.Web.Catalog;

/// <summary>EF Core implementation of <see cref="IProductCatalog"/> over the catalog tables.</summary>
public sealed class ProductCatalog(IDbContextFactory<AppDbContext> dbFactory) : IProductCatalog
{
	private static readonly Expression<Func<Data.Product, Product>> ToView = p => new Product(
		p.Slug,
		p.Name,
		p.Category.Name,
		p.Price,
		p.ImageId,
		p.Description,
		// The stock row is an optional dependent; a product without one reads as sold out.
		p.Stock == null ? 0 : p.Stock.Quantity,
		p.Badge,
		p.WasPrice);

	/// <summary>The most recently added products, newest first.</summary>
	public async Task<IReadOnlyList<Product>> GetNewArrivalsAsync(int count, CancellationToken cancellationToken = default)
	{
		await using AppDbContext db = await dbFactory.CreateDbContextAsync(cancellationToken);
		return await db.Products
			.OrderByDescending(p => p.CreatedAt)
			.Take(count)
			.Select(ToView)
			.ToListAsync(cancellationToken);
	}

	/// <summary>Finds a product by its URL slug (case-insensitive), or returns <see langword="null"/> when none matches.</summary>
	public async Task<Product?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		string normalized = slug.ToLowerInvariant();
		await using AppDbContext db = await dbFactory.CreateDbContextAsync(cancellationToken);
		return await db.Products
			.Where(p => p.Slug == normalized)
			.Select(ToView)
			.FirstOrDefaultAsync(cancellationToken);
	}

	/// <summary>Other products to suggest alongside <paramref name="slug"/>: same category first, then newest.</summary>
	public async Task<IReadOnlyList<Product>> GetRelatedAsync(string slug, int count, CancellationToken cancellationToken = default)
	{
		string normalized = slug.ToLowerInvariant();
		await using AppDbContext db = await dbFactory.CreateDbContextAsync(cancellationToken);
		IQueryable<int> categoryId = db.Products.Where(p => p.Slug == normalized).Select(p => p.CategoryId);
		return await db.Products
			.Where(p => p.Slug != normalized)
			.OrderByDescending(p => categoryId.Contains(p.CategoryId))
			.ThenByDescending(p => p.CreatedAt)
			.Take(count)
			.Select(ToView)
			.ToListAsync(cancellationToken);
	}
}
