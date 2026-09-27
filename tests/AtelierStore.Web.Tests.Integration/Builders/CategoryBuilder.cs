using AtelierStore.Web.Data;

namespace AtelierStore.Web.Tests.Integration.Builders;

/// <summary>Builds a minimal <see cref="Category"/> row for a test to insert directly.</summary>
public sealed class CategoryBuilder
{
	private string _slug = "outerwear";

	private string _name = "Outerwear";

	public CategoryBuilder WithSlug(string slug)
	{
		_slug = slug;
		return this;
	}

	public CategoryBuilder WithName(string name)
	{
		_name = name;
		return this;
	}

	public Category Build() => new() { Slug = _slug, Name = _name };
}
