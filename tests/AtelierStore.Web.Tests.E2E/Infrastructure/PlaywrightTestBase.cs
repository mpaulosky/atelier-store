using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace AtelierStore.Web.Tests.E2E.Infrastructure;

/// <summary>
/// Opens a fresh <see cref="IBrowserContext"/>/<see cref="IPage"/> before each test and closes it after,
/// so tests never leak cookies, storage or navigation history between each other. On failure (when CI sets
/// <c>PLAYWRIGHT_ARTIFACTS=true</c>), saves a trace and a screenshot for debugging.
/// </summary>
[Collection(E2ECollection.Name)]
public abstract class PlaywrightTestBase(E2EFixture fixture) : IAsyncLifetime
{
	private static readonly Regex InvalidFileNameChars = new("[^A-Za-z0-9_-]+", RegexOptions.Compiled);

	private static bool CaptureArtifacts =>
		string.Equals(Environment.GetEnvironmentVariable("PLAYWRIGHT_ARTIFACTS"), "true", StringComparison.OrdinalIgnoreCase);

	protected E2EFixture Fixture { get; } = fixture;

	protected IBrowserContext Context { get; private set; } = null!;

	protected IPage Page { get; private set; } = null!;

	/// <summary>Override to customize the context (e.g. a mobile viewport or a signed-in cookie).</summary>
	protected virtual Task<IBrowserContext> CreateContextAsync() => Fixture.NewContextAsync();

	public async ValueTask InitializeAsync()
	{
		Context = await CreateContextAsync();
		if (CaptureArtifacts)
		{
			await Context.Tracing.StartAsync(new TracingStartOptions { Screenshots = true, Snapshots = true, Sources = true });
		}

		Page = await Context.NewPageAsync();
	}

	public async ValueTask DisposeAsync()
	{
		bool failed = TestContext.Current.TestState?.Result == TestResult.Failed;

		if (CaptureArtifacts)
		{
			string artifactsDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "playwright-artifacts");
			string testName = InvalidFileNameChars.Replace(TestContext.Current.Test?.TestDisplayName ?? GetType().Name, "_");

			if (failed)
			{
				Directory.CreateDirectory(artifactsDirectory);
				await Context.Tracing.StopAsync(new TracingStopOptions { Path = Path.Combine(artifactsDirectory, $"{testName}.trace.zip") });
				await Page.ScreenshotAsync(new PageScreenshotOptions { Path = Path.Combine(artifactsDirectory, $"{testName}.png"), FullPage = true });
			}
			else
			{
				await Context.Tracing.StopAsync();
			}
		}

		await Context.CloseAsync();
	}
}
