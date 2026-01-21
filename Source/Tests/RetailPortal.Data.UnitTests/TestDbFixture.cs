using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailPortal.Data.Db.Context;

namespace RetailPortal.Infrastructure.UnitTests;

public class TestDbFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public IDbContextFactory<ApplicationDbContext> ContextFactory { get; }

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

    public void Dispose() => this._serviceProvider.Dispose();
}
