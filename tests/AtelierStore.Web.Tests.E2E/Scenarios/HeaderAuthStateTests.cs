using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 6 (anonymous half): the header offers "Log in" when no one is signed in.</summary>
public sealed class AnonymousHeaderTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task AnonymousVisitor_SeesLogInLink()
	{
		// Arrange & Act
		await Page.GotoAsync("/");

		// Assert
		await Expect(Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Log in" })).ToBeVisibleAsync();
	}
}

/// <summary>Scenario 6 (signed-in half): the header offers "Log out" once a visitor is signed in via the fake auth scheme.</summary>
public sealed class SignedInHeaderTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	protected override Task<IBrowserContext> CreateContextAsync() => Fixture.NewContextAsync(signedIn: true);

	[Fact]
	public async Task SignedInVisitor_SeesLogOutLink()
	{
		// Arrange & Act
		await Page.GotoAsync("/");

		// Assert
		await Expect(Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Log out" })).ToBeVisibleAsync();
	}
}
