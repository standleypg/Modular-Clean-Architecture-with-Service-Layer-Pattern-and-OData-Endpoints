namespace RetailPortal.Infrastructure.UnitTests.Common;

/// <summary>
/// Base class for tests that need isolated database per test method.
/// Each test gets a fresh in-memory database.
/// Use when tests depend on specific data counts or need clean state.
/// </summary>
public abstract class IsolatedDatabaseTestBase : IAsyncLifetime, IDisposable
{
    protected TestDbFixture Fixture { get; private set; } = null!;
    protected RepositoryUtils RepositoryUtils { get; private set; } = null!;

    public virtual Task InitializeAsync()
    {
        this.Fixture = new TestDbFixture();
        this.RepositoryUtils = new RepositoryUtils(this.Fixture.SharedContext);
        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        this.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Fixture?.Dispose();
        }
    }
}
