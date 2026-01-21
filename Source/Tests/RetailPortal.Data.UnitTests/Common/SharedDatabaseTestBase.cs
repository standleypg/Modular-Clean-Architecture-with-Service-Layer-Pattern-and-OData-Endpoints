namespace RetailPortal.Infrastructure.UnitTests.Common;

/// <summary>
/// Base class for tests that share database across all test methods in the class.
/// Faster execution but tests may affect each other's data.
/// Use when tests are independent of data state or don't assert on counts.
/// </summary>
public abstract class SharedDatabaseTestBase(TestDbFixture fixture) : IClassFixture<TestDbFixture>
{
    protected TestDbFixture Fixture { get; } = fixture;
    protected RepositoryUtils RepositoryUtils { get; } = new(fixture.ContextFactory);
}
