using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailPortal.Data.Db.Context;

namespace RetailPortal.Infrastructure.UnitTests;

public class TestDbFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private ApplicationDbContext? _sharedContext;

    public IDbContextFactory<ApplicationDbContext> ContextFactory { get; }

    /// <summary>
    /// A shared context that lives for the duration of the test.
    /// Use this for operations where you need the context to remain open.
    /// </summary>
    public ApplicationDbContext SharedContext => this._sharedContext ??= this.ContextFactory.CreateDbContext();

    public TestDbFixture()
    {
        var services = new ServiceCollection();

        services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
        });

        this._serviceProvider = services.BuildServiceProvider();
        this.ContextFactory = this._serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    }

    public void Dispose()
    {
        this._sharedContext?.Dispose();
        this._serviceProvider.Dispose();
    }
}
