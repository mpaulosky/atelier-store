namespace AtelierStore.Web.Data;

public class Category
{
	public int Id { get; set; }

	/// <summary>Lowercase URL key, unique across categories.</summary>
	public required string Slug { get; set; }

	public required string Name { get; set; }

	public List<Product> Products { get; set; } = [];
}
