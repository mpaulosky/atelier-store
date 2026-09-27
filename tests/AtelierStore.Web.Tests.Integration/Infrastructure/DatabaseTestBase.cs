namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// Resets the shared database to empty (via Respawn) before every test, so each test builds exactly the
/// rows it needs with the builders in <c>Builders/</c> instead of relying on the seeded starter catalog.
/// </summary>
[Collection(DatabaseCollection.Name)]
public abstract class DatabaseTestBase(PostgresContainerFixture fixture) : IAsyncLifetime
{
	protected PostgresContainerFixture Fixture { get; } = fixture;

	public async ValueTask InitializeAsync() => await Fixture.ResetDatabaseAsync();

	public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
