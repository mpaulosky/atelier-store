using AtelierStore.Web.Data;
using AtelierStore.Web.Tests.Integration.Builders;
using AtelierStore.Web.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AtelierStore.Web.Tests.Integration;

/// <summary>The catalog's check constraints, verified against real Postgres.</summary>
public sealed class DatabaseConstraintTests(PostgresContainerFixture fixture) : DatabaseTestBase(fixture)
{
	[Fact]
	public async Task SaveChangesAsync_NegativePrice_ThrowsDbUpdateException()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder().WithSlug("negative-price").InCategory(category.Id).WithPrice(-1m).Build();
		db.Products.Add(product);

		// Act
		Func<Task> act = async () => await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Assert
		await act.Should().ThrowAsync<DbUpdateException>();
	}

	[Fact]
	public async Task SaveChangesAsync_WasPriceNotAbovePrice_ThrowsDbUpdateException()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder()
			.WithSlug("was-price-not-above-price")
			.InCategory(category.Id)
			.WithPrice(100m)
			.WithWasPrice(100m) // must be strictly greater than price, not equal
			.Build();
		db.Products.Add(product);

		// Act
		Func<Task> act = async () => await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Assert
		await act.Should().ThrowAsync<DbUpdateException>();
	}

	[Fact]
	public async Task SaveChangesAsync_NegativeStockQuantity_ThrowsDbUpdateException()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder().WithSlug("negative-stock").InCategory(category.Id).Build();
		db.Products.Add(product);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		ProductStock stock = new ProductStockBuilder().ForProduct(product.Id).WithQuantity(-1).Build();
		db.ProductStock.Add(stock);

		// Act
		Func<Task> act = async () => await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Assert
		await act.Should().ThrowAsync<DbUpdateException>();
	}

	[Fact]
	public async Task SaveChangesAsync_ProductSlugWithUppercase_ThrowsDbUpdateException()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		// FindBySlugAsync lowercases the search term, so a stored uppercase slug could never be found (#16).
		Product product = new ProductBuilder().WithSlug("Mixed-Case").InCategory(category.Id).Build();
		db.Products.Add(product);

		// Act
		Func<Task> act = async () => await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Assert
		await act.Should().ThrowAsync<DbUpdateException>();
	}

	[Fact]
	public async Task SaveChangesAsync_CategorySlugWithUppercase_ThrowsDbUpdateException()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().WithSlug("Mixed-Case").Build();
		db.Categories.Add(category);

		// Act
		Func<Task> act = async () => await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Assert
		await act.Should().ThrowAsync<DbUpdateException>();
	}
}
