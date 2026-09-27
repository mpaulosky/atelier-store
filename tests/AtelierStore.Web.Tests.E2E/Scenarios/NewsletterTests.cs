using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 4: the home page newsletter form validates the email and acknowledges a valid one.</summary>
public sealed class NewsletterTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task InvalidEmail_BlocksSubmission_BeforeItReachesTheServer()
	{
		// Arrange
		await Page.GotoAsync("/");
		ILocator emailInput = Page.GetByLabel("Email address");
		await emailInput.FillAsync("not-an-email");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Subscribe" }).ClickAsync();

		// Assert
		// BUG: #17 - the input is type="email", so a compliant browser's own HTML5 constraint
		// validation blocks the submit before the 'submit' event (and so Blazor's enhanced-nav handler)
		// ever fires. NewsletterSignup.Email's [EmailAddress] "Enter a valid email address." message is
		// therefore unreachable through real keyboard/mouse use: any string a browser lets through already
		// satisfies EmailAddressAttribute's own permissive "exactly one '@', not at either end" check.
		bool nativelyInvalid = await emailInput.EvaluateAsync<bool>("el => !el.checkValidity()");
		nativelyInvalid.Should().BeTrue();
		await Expect(Page.GetByText("Enter a valid email address.")).Not.ToBeVisibleAsync();
		await Expect(Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Subscribe" })).ToBeVisibleAsync();
	}

	[Fact]
	public async Task ValidEmail_ShowsSubscribedConfirmation()
	{
		// Arrange
		await Page.GotoAsync("/");
		await Page.GetByLabel("Email address").FillAsync("client@example.com");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Subscribe" }).ClickAsync();

		// Assert
		await Expect(Page.GetByText("You're on the list.")).ToBeVisibleAsync();
	}
}
