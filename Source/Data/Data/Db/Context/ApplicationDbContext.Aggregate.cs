using Microsoft.EntityFrameworkCore;
using RetailPortal.Data.Db.Repositories;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Context;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    internal AggregateRepository<User> Users => new(
        this.Set<User>(),
        this.Set<User>()
            .Include(u => u.Addresses)
            .Include(u => u.Roles)
    );

    internal AggregateRepository<Product> Products => new(
        this.Set<Product>(),
        this.Set<Product>()
            .Include(p => this.Users)
    );

    internal AggregateRepository<Role> Roles => new(
        this.Set<Role>()
    );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}