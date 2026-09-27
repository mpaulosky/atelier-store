using AtelierStore.Web.Data;
using AtelierStore.Web.Tests.Integration.Builders;
using AtelierStore.Web.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CatalogProduct = AtelierStore.Web.Catalog.Product;
using ProductCatalog = AtelierStore.Web.Catalog.ProductCatalog;
using IProductCatalog = AtelierStore.Web.Catalog.IProductCatalog;
using StockState = AtelierStore.Web.Catalog.StockState;

namespace AtelierStore.Web.Tests.Integration;

/// <summary>
/// <see cref="ProductCatalog"/> against a real Postgres database. Each test builds exactly the rows it
/// needs (the database is reset to empty before every test, see <see cref="DatabaseTestBase"/>) and reads
/// them back only through <see cref="IProductCatalog"/>, the same seam the storefront pages use.
/// </summary>
public sealed class ProductCatalogTests(PostgresContainerFixture fixture) : DatabaseTestBase(fixture)
{
	private readonly IProductCatalog _catalog = new ProductCatalog(fixture.CreateDbContextFactory());

	[Fact]
	public async Task FindBySlugAsync_DifferentCaseThanTheStoredLowercaseSlug_FindsTheProduct()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder().WithSlug("wool-coat").InCategory(category.Id).Build();
		db.Products.Add(product);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		CatalogProduct? found = await _catalog.FindBySlugAsync("WOOL-COAT", TestContext.Current.CancellationToken);

		// Assert
		found.Should().NotBeNull();
		found!.Slug.Should().Be("wool-coat");
	}

	[Fact]
	public async Task FindBySlugAsync_ProductStoredWithAMixedCaseSlug_CannotFindItByItsOwnSlug()
	{
		// BUG: #16 - nothing stops a product being stored with a non-lowercase slug, and
		// FindBySlugAsync only lowercases the *search* term, not the stored value, so a
		// mixed-case row can never be found again, even by its own exact slug.
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder().WithSlug("Mixed-Case").InCategory(category.Id).Build();
		db.Products.Add(product);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		CatalogProduct? found = await _catalog.FindBySlugAsync("Mixed-Case", TestContext.Current.CancellationToken);

		// Assert
		found.Should().BeNull();
	}

	[Fact]
	public async Task FindBySlugAsync_UnknownSlug_ReturnsNull()
	{
		// Arrange

		// Act
		CatalogProduct? found = await _catalog.FindBySlugAsync("does-not-exist", TestContext.Current.CancellationToken);

		// Assert
		found.Should().BeNull();
	}

	[Fact]
	public async Task FindBySlugAsync_ProductWithoutAStockRow_ReadsAsZeroStockAndSoldOut()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product product = new ProductBuilder().WithSlug("no-stock-row").InCategory(category.Id).Build();
		db.Products.Add(product);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		// Deliberately no ProductStock row for this product.

		// Act
		CatalogProduct? found = await _catalog.FindBySlugAsync("no-stock-row", TestContext.Current.CancellationToken);

		// Assert
		found.Should().NotBeNull();
		found!.Stock.Should().Be(0);
		found.StockState.Should().Be(StockState.SoldOut);
	}

	[Fact]
	public async Task GetRelatedAsync_IncludesTheProductItself_ExcludesIt()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product target = new ProductBuilder().WithSlug("target").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 3, 0, 0, 0, TimeSpan.Zero)).Build();
		Product other = new ProductBuilder().WithSlug("other").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)).Build();
		db.Products.AddRange(target, other);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> related = await _catalog.GetRelatedAsync("target", count: 10, TestContext.Current.CancellationToken);

		// Assert
		related.Should().ContainSingle();
		related[0].Slug.Should().Be("other");
	}

	[Fact]
	public async Task GetRelatedAsync_SameCategoryProducts_ComeBeforeOtherCategories()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category categoryA = new CategoryBuilder().WithSlug("category-a").WithName("Category A").Build();
		Category categoryB = new CategoryBuilder().WithSlug("category-b").WithName("Category B").Build();
		db.Categories.AddRange(categoryA, categoryB);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		Product target = new ProductBuilder().WithSlug("target").InCategory(categoryA.Id).CreatedAt(new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero)).Build();
		// Newer, but a different category: must still sort after the same-category match.
		Product newerOtherCategory = new ProductBuilder().WithSlug("newer-other-category").InCategory(categoryB.Id).CreatedAt(new DateTimeOffset(2026, 1, 4, 0, 0, 0, TimeSpan.Zero)).Build();
		Product olderSameCategory = new ProductBuilder().WithSlug("older-same-category").InCategory(categoryA.Id).CreatedAt(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)).Build();
		db.Products.AddRange(target, newerOtherCategory, olderSameCategory);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> related = await _catalog.GetRelatedAsync("target", count: 10, TestContext.Current.CancellationToken);

		// Assert
		related.Select(p => p.Slug).Should().Equal("older-same-category", "newer-other-category");
	}

	[Fact]
	public async Task GetRelatedAsync_WithinACategory_OrdersNewestFirst()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		Product target = new ProductBuilder().WithSlug("target").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero)).Build();
		Product older = new ProductBuilder().WithSlug("older").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)).Build();
		Product newer = new ProductBuilder().WithSlug("newer").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero)).Build();
		db.Products.AddRange(target, older, newer);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> related = await _catalog.GetRelatedAsync("target", count: 10, TestContext.Current.CancellationToken);

		// Assert
		related.Select(p => p.Slug).Should().Equal("newer", "older");
	}

	[Fact]
	public async Task GetRelatedAsync_HonoursCount_ReturnsNoMoreThanRequested()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product target = new ProductBuilder().WithSlug("target").InCategory(category.Id).Build();
		db.Products.Add(target);
		for (int i = 0; i < 5; i++)
		{
			db.Products.Add(new ProductBuilder().WithSlug($"other-{i}").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 1 + i, 0, 0, 0, TimeSpan.Zero)).Build());
		}

		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> related = await _catalog.GetRelatedAsync("target", count: 2, TestContext.Current.CancellationToken);

		// Assert
		related.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetNewArrivalsAsync_OrdersNewestCreatedAtFirst()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		Product oldest = new ProductBuilder().WithSlug("oldest").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)).Build();
		Product middle = new ProductBuilder().WithSlug("middle").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)).Build();
		Product newest = new ProductBuilder().WithSlug("newest").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 3, 0, 0, 0, TimeSpan.Zero)).Build();
		db.Products.AddRange(oldest, middle, newest);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> arrivals = await _catalog.GetNewArrivalsAsync(count: 10, TestContext.Current.CancellationToken);

		// Assert
		arrivals.Select(p => p.Slug).Should().Equal("newest", "middle", "oldest");
	}

	[Fact]
	public async Task GetNewArrivalsAsync_HonoursCount_ReturnsNoMoreThanRequested()
	{
		// Arrange
		await using AppDbContext db = await Fixture.CreateDbContextFactory().CreateDbContextAsync(TestContext.Current.CancellationToken);
		Category category = new CategoryBuilder().Build();
		db.Categories.Add(category);
		await db.SaveChangesAsync(TestContext.Current.CancellationToken);
		for (int i = 0; i < 5; i++)
		{
			db.Products.Add(new ProductBuilder().WithSlug($"arrival-{i}").InCategory(category.Id).CreatedAt(new DateTimeOffset(2026, 1, 1 + i, 0, 0, 0, TimeSpan.Zero)).Build());
		}

		await db.SaveChangesAsync(TestContext.Current.CancellationToken);

		// Act
		IReadOnlyList<CatalogProduct> arrivals = await _catalog.GetNewArrivalsAsync(count: 3, TestContext.Current.CancellationToken);

		// Assert
		arrivals.Should().HaveCount(3);
	}
}
