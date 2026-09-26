using AtelierStore.Web.Catalog;

namespace AtelierStore.Web.Tests.Unit.Catalog;

public class UnsplashImageTests
{

	[Fact]
	public void Url_GivenImageIdAndWidth_BuildsResizedUnsplashUrl()
	{

		// Arrange
		const string imageId = "1483985988355-763728e1935b";
		const int width = 640;

		// Act
		string actual = UnsplashImage.Url(imageId, width);

		// Assert
		actual.Should().Be("https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&q=80&w=640");

	}

	[Fact]
	public void SrcSet_GivenImageIdAndWidths_BuildsCommaSeparatedSrcSet()
	{

		// Arrange
		const string imageId = "1483985988355-763728e1935b";

		// Act
		string actual = UnsplashImage.SrcSet(imageId, 320, 640);

		// Assert
		actual.Should().Be(
			"https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&q=80&w=320 320w, " +
			"https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&q=80&w=640 640w");

	}

	[Fact]
	public void SrcSet_GivenNoWidths_ReturnsEmptyString()
	{

		// Arrange
		const string imageId = "1483985988355-763728e1935b";

		// Act
		string actual = UnsplashImage.SrcSet(imageId);

		// Assert
		actual.Should().BeEmpty();

	}

}
