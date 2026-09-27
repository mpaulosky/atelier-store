namespace AtelierStore.Web.Tests.E2E.Infrastructure;

/// <summary>
/// Every E2E test class shares one <see cref="E2EFixture"/> (one database, one browser) and runs
/// sequentially: parallel navigation in the same browser/database would make tests flaky.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class E2ECollection : ICollectionFixture<E2EFixture>
{
	public const string Name = "E2E";
}
