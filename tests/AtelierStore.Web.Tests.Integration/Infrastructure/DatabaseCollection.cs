namespace AtelierStore.Web.Tests.Integration.Infrastructure;

/// <summary>
/// Every test class that touches the shared Postgres container joins this collection so xUnit runs them
/// sequentially, never in parallel with each other, against the one container.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class DatabaseCollection
{
	public const string Name = "Database";
}
