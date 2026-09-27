using AtelierStore.Web.Data;
using AtelierStore.Web.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(PostgresContainerFixture))]

namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// One Postgres 17 container for the whole assembly (declared via <c>[assembly: AssemblyFixture]</c> in
/// <see cref="AssemblyFixtures"/>). Migrates the real schema once, captures a <see cref="Seed"/> snapshot
/// of what <c>HasData</c> seeded before any test can touch the database, then resets between tests with
/// Respawn so each test starts from empty tables and builds its own rows.
/// </summary>
public sealed class PostgresContainerFixture : IAsyncLifetime
{
	private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17")
		.WithDatabase("atelier_store_test")
		.WithUsername("atelier")
		.WithPassword("atelier")
		.Build();

	private Respawner? _respawner;

	/// <summary>The container's connection string. Every test host and test-side <see cref="AppDbContext"/> uses this.</summary>
	public string ConnectionString { get; private set; } = string.Empty;

	/// <summary>What the InitialCatalog migration seeded, captured before any reset. See <see cref="SeedSnapshot"/>.</summary>
	public SeedSnapshot Seed { get; private set; } = null!;

	public async ValueTask InitializeAsync()
	{
		// Set these as real process environment variables *before* Program.cs ever runs. Program.cs's
		// `DotNetEnv.Env.NoClobber().TraversePath().Load()` walks up from the test binary's directory and
		// would find the developer's real .env (it sits above this worktree) with a real Auth0 tenant and
		// real database connection string. NoClobber means it never overwrites a variable that is already
		// set, so setting these first is what actually keeps the real Auth0 tenant and the developer's
		// database out of every test, regardless of how ASP.NET Core's configuration precedence resolves
		// later `builder.UseSetting(...)` calls in AtelierWebApplicationFactory.
		Environment.SetEnvironmentVariable("Auth0__Domain", TestAuth0.FakeDomain);
		Environment.SetEnvironmentVariable("Auth0__ClientId", TestAuth0.FakeClientId);

		await _container.StartAsync();
		ConnectionString = _container.GetConnectionString();
		Environment.SetEnvironmentVariable("ConnectionStrings__Default", ConnectionString);

		DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(ConnectionString)
			.UseSnakeCaseNamingConvention()
			.Options;

		await using AppDbContext db = new(options);
		await db.Database.MigrateAsync(CancellationToken.None);

		Seed = new SeedSnapshot(
			CategoryCount: await db.Categories.CountAsync(CancellationToken.None),
			ProductCount: await db.Products.CountAsync(CancellationToken.None),
			ProductSlugsNewestFirst: await db.Products
				.OrderByDescending(p => p.CreatedAt)
				.Select(p => p.Slug)
				.ToListAsync(CancellationToken.None),
			SoldOutProductSlugs: await db.Products
				.Where(p => p.Stock == null || p.Stock.Quantity == 0)
				.Select(p => p.Slug)
				.ToListAsync(CancellationToken.None));
	}

	public async ValueTask DisposeAsync()
	{
		await _container.DisposeAsync();
	}

	/// <summary>Wipes every table (except migration history) so the next test starts from an empty database.</summary>
	public async Task ResetDatabaseAsync()
	{
		await using NpgsqlConnection connection = new(ConnectionString);
		await connection.OpenAsync();

		_respawner ??= await Respawner.CreateAsync(connection, new RespawnerOptions
		{
			DbAdapter = DbAdapter.Postgres,
			SchemasToInclude = ["public"],
			TablesToIgnore = [new Table("__EFMigrationsHistory")],
		});

		await _respawner.ResetAsync(connection);
	}

	/// <summary>Builds a standalone context factory pointed at the container, for tests that exercise <c>ProductCatalog</c> directly.</summary>
	public IDbContextFactory<AppDbContext> CreateDbContextFactory() => new TestDbContextFactory(ConnectionString);

	private sealed class TestDbContextFactory(string connectionString) : IDbContextFactory<AppDbContext>
	{
		private readonly DbContextOptions<AppDbContext> _options = new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(connectionString)
			.UseSnakeCaseNamingConvention()
			.Options;

		public AppDbContext CreateDbContext() => new(_options);

		public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
			Task.FromResult(CreateDbContext());
	}
}
