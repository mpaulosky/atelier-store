using AtelierStore.Web.Tests.Integration.Infrastructure;

namespace AtelierStore.Web.Tests.Integration;

/// <summary>
/// Asserts what the InitialCatalog migration seeds, using the snapshot <see cref="PostgresContainerFixture"/>
/// captures immediately after migrating and before any test's Respawn reset runs (Respawn wipes the seed,
/// so a live query here would see whatever the most recently run test left behind instead).
/// </summary>
[Collection(DatabaseCollection.Name)]
public sealed class SeedDataTests(PostgresContainerFixture fixture)
{
	[Fact]
	public void Migrations_SeedTheStarterCatalog_InsertsSevenCategoriesAndEightProducts()
	{
		// Arrange
		SeedSnapshot seed = fixture.Seed;

		// Act & Assert
		seed.CategoryCount.Should().Be(7);
		seed.ProductCount.Should().Be(8);
	}

	[Fact]
	public void Migrations_SeedTheStarterCatalog_OrdersProductsNewestCreatedAtFirst()
	{
		// Arrange
		SeedSnapshot seed = fixture.Seed;

		// Act
		IReadOnlyList<string> newestFirst = seed.ProductSlugsNewestFirst;

		// Assert
		newestFirst.Should().Equal(
			"leather-biker-jacket",
			"hand-knit-poncho",
			"silk-jogger",
			"top-handle-bag",
			"botanical-tote",
			"panelled-runner-sneaker",
			"round-metal-sunglasses",
			"pearl-collar-necklace");
	}

	[Fact]
	public void Migrations_SeedTheStarterCatalog_LeavesExactlyOneProductSoldOut()
	{
		// Arrange
		SeedSnapshot seed = fixture.Seed;

		// Act & Assert
		seed.SoldOutProductSlugs.Should().ContainSingle().Which.Should().Be("botanical-tote");
	}
}
