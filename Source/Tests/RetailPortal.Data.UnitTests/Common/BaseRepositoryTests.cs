using Microsoft.EntityFrameworkCore;
using RetailPortal.Data.Db.Context;

namespace RetailPortal.Infrastructure.UnitTests.Common;

public class BaseRepositoryTests
{
    public ApplicationDbContext Context { get; }

    protected BaseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"RetailPortal-{Guid.NewGuid().ToString()}")
            .Options;

        this.Context = new ApplicationDbContext(options);

        this.Context.Database.EnsureCreated();
        this.Context.Database.EnsureDeleted();
    }
}