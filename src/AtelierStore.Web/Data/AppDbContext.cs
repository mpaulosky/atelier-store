using Microsoft.EntityFrameworkCore;

namespace AtelierStore.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Category> Categories => Set<Category>();

	public DbSet<Product> Products => Set<Product>();

	public DbSet<ProductStock> ProductStock => Set<ProductStock>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>(category =>
		{
			category.Property(c => c.Slug).HasMaxLength(100);
			category.Property(c => c.Name).HasMaxLength(100);
			category.HasIndex(c => c.Slug).IsUnique();
			category.HasData(CatalogSeedData.Categories);
		});

		modelBuilder.Entity<Product>(product =>
		{
			product.ToTable(table =>
			{
				table.HasCheckConstraint("ck_products_price_non_negative", "price >= 0");
				table.HasCheckConstraint("ck_products_was_price_above_price", "was_price IS NULL OR was_price > price");
			});
			product.Property(p => p.Slug).HasMaxLength(100);
			product.Property(p => p.Name).HasMaxLength(200);
			product.Property(p => p.Price).HasPrecision(10, 2);
			product.Property(p => p.WasPrice).HasPrecision(10, 2);
			product.Property(p => p.ImageId).HasMaxLength(100);
			product.Property(p => p.Badge).HasMaxLength(40);
			product.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
			product.HasIndex(p => p.Slug).IsUnique();
			product.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);
			product.HasData(CatalogSeedData.Products);
		});

		modelBuilder.Entity<ProductStock>(stock =>
		{
			stock.ToTable(table => table.HasCheckConstraint("ck_product_stock_quantity_non_negative", "quantity >= 0"));
			stock.HasKey(s => s.ProductId);
			stock.Property(s => s.UpdatedAt).HasDefaultValueSql("now()");
			stock.HasOne(s => s.Product)
				.WithOne(p => p.Stock)
				.HasForeignKey<ProductStock>(s => s.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
			stock.HasData(CatalogSeedData.Stock);
		});
	}
}
