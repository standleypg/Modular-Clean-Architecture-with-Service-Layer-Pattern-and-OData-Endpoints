namespace RetailPortal.Infrastructure.UnitTests.Common;

/// <summary>
/// Base class for tests that share database across all test methods in the class.
/// Faster execution but tests may affect each other's data.
/// Use when tests are independent of data state or don't assert on counts.
/// </summary>
public abstract class SharedDatabaseTestBase : IClassFixture<TestDbFixture>
{
    protected TestDbFixture Fixture { get; }
    protected RepositoryUtils RepositoryUtils { get; }

    protected SharedDatabaseTestBase()
    {
        this.Fixture = new TestDbFixture();
        this.RepositoryUtils = new RepositoryUtils(this.Fixture.SharedContext);
    }
}
