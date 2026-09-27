using AtelierStore.Web.Tests.E2E.Infrastructure;

namespace AtelierStore.Web.Tests.E2E.Scenarios;

/// <summary>Scenario 4: the home page newsletter form validates the email and acknowledges a valid one.</summary>
public sealed class NewsletterTests(E2EFixture fixture) : PlaywrightTestBase(fixture)
{
	[Fact]
	public async Task InvalidEmail_ShowsServerValidationMessage()
	{
		// Arrange
		await Page.GotoAsync("/");
		await Page.GetByLabel("Email address").FillAsync("not-an-email");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Subscribe" }).ClickAsync();

		// Assert
		// #17: the form is novalidate, so the browser posts it and the server's [EmailAddress] message renders.
		await Expect(Page.GetByText("Enter a valid email address.")).ToBeVisibleAsync();
		await Expect(Page.GetByText("You're on the list.")).Not.ToBeVisibleAsync();
	}

	[Fact]
	public async Task EmptyEmail_ShowsRequiredMessage()
	{
		// Arrange
		await Page.GotoAsync("/");

		// Act
		await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Subscribe" }).ClickAsync();

		// Assert
		await Expect(Page.GetByText("Enter your email address.")).ToBeVisibleAsync();
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
