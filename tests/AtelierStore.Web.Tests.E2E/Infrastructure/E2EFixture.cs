using AtelierStore.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Testcontainers.PostgreSql;

namespace AtelierStore.Web.Tests.E2E.Infrastructure;

/// <summary>
/// One assembly-wide Postgres container, storefront host and Chromium browser, shared by every E2E test
/// (see <see cref="E2ECollection"/>, which also disables parallelization so tests never race for the
/// one browser / one database).
/// </summary>
public sealed class E2EFixture : IAsyncLifetime
{
	/// <summary>Slug of the E2E-owned in-stock product (not part of the merchandised "New in" seed data).</summary>
	public const string InStockSlug = "e2e-signature-trench";

	public const string InStockName = "E2E Signature Trench";

	public const decimal InStockPrice = 1200m;

	/// <summary>Slug of the E2E-owned sold-out product (no stock row at all, which the catalog reads as zero).</summary>
	public const string SoldOutSlug = "e2e-heritage-tweed-scarf";

	public const string SoldOutName = "E2E Heritage Tweed Scarf";

	private const int InStockProductId = 9001;
	private const int SoldOutProductId = 9002;
	private const int ReusedCategoryId = 1; // "outerwear", seeded by CatalogSeedData.

	private PostgreSqlContainer database = null!;
	private AtelierAppFactory factory = null!;
	private IPlaywright playwright = null!;

	/// <summary>The Chromium browser shared by every test in the collection; each test opens its own context.</summary>
	public IBrowser Browser { get; private set; } = null!;

	/// <summary>The base URL of the in-process Kestrel listener hosting the storefront.</summary>
	public string BaseUrl => factory.ServerAddress;

	/// <summary>
	/// Opens a fresh browser context pointed at the storefront. When <paramref name="signedIn"/> is
	/// <see langword="true"/>, the context carries the cookie <see cref="TestAuthHandler"/> reads, so the
	/// storefront treats every request in that context as an authenticated visitor without ever
	/// contacting Auth0.
	/// </summary>
	public async Task<IBrowserContext> NewContextAsync(bool signedIn = false, BrowserNewContextOptions? options = null)
	{
		options ??= new BrowserNewContextOptions();
		options.BaseURL = BaseUrl;

		IBrowserContext context = await Browser.NewContextAsync(options);

		if (signedIn)
		{
			await context.AddCookiesAsync(
			[
				new Cookie
				{
					Name = TestAuthHandler.CookieName,
					Value = TestAuthHandler.CookieValue,
					Url = BaseUrl,
				},
			]);
		}

		return context;
	}

	public async ValueTask InitializeAsync()
	{
		database = new PostgreSqlBuilder("postgres:17").Build();
		await database.StartAsync();

		string connectionString = database.GetConnectionString();

		// Defense in depth: Program.cs loads a developer .env with DotNetEnv.NoClobber(), which would
		// otherwise leave real Auth0/Postgres settings in place if this process's environment doesn't
		// already claim those keys. Claiming them here means the E2E host can never reach out for real.
		Environment.SetEnvironmentVariable("Auth0__Domain", "e2e-tests.example.com");
		Environment.SetEnvironmentVariable("Auth0__ClientId", "e2e-test-client-id");
		Environment.SetEnvironmentVariable("Auth0__ClientSecret", "e2e-test-client-secret");
		Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);

		await MigrateAndSeedAsync(connectionString);

		factory = new AtelierAppFactory(connectionString);
		await factory.StartOnKestrelAsync();

		playwright = await Playwright.CreateAsync();
		Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
	}

	public async ValueTask DisposeAsync()
	{
		await Browser.CloseAsync();
		playwright.Dispose();
		await factory.DisposeAsync();
		await database.DisposeAsync();
	}

	private static async Task MigrateAndSeedAsync(string connectionString)
	{
		DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(connectionString)
			.UseSnakeCaseNamingConvention()
			.Options;

		await using AppDbContext db = new(options);
		await db.Database.MigrateAsync();

		db.Products.Add(new Product
		{
			Id = InStockProductId,
			Slug = InStockSlug,
			Name = InStockName,
			Description = "An E2E-owned fixture product: always in stock, so the add-to-bag scenario never depends on merchandising seed data.",
			CategoryId = ReusedCategoryId,
			Price = InStockPrice,
			ImageId = "1551028719-00167b16eac5",
			CreatedAt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
			Stock = new ProductStock { Quantity = 25, UpdatedAt = DateTimeOffset.UtcNow },
		});

		db.Products.Add(new Product
		{
			Id = SoldOutProductId,
			Slug = SoldOutSlug,
			Name = SoldOutName,
			Description = "An E2E-owned fixture product with no stock row, so the catalog always reads it as sold out.",
			CategoryId = ReusedCategoryId,
			Price = 640m,
			ImageId = "1434389677669-e08b4cac3105",
			CreatedAt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
		});

		await db.SaveChangesAsync();
	}
}
