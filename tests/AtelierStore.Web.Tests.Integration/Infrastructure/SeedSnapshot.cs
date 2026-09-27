namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// What the InitialCatalog migration's <c>HasData</c> seeded, captured once by
/// <see cref="PostgresContainerFixture"/> immediately after migrating and before any test resets the
/// database. Later Respawn resets wipe this seed, so tests that want to assert on it read this snapshot
/// instead of querying the (by-then-empty) tables.
/// </summary>
public sealed record SeedSnapshot(
	int CategoryCount,
	int ProductCount,
	IReadOnlyList<string> ProductSlugsNewestFirst,
	IReadOnlyList<string> SoldOutProductSlugs);
