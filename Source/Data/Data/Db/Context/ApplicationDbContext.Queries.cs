using Microsoft.EntityFrameworkCore;
using RetailPortal.Data.Db.Repositories;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Context;

public partial class ApplicationDbContext
{
    internal IReadOnlyRepository<Address> AddressesQueries => new ReadOnlyRepository<Address>(
        this.Set<Address>()
    );
    internal IReadOnlyRepository<Role> RolesQueries => new ReadOnlyRepository<Role>(
        this.Set<Role>()
    );
    internal IReadOnlyRepository<User> UsersQueries => new ReadOnlyRepository<User>(
        this.Set<User>()
    );
    internal IReadOnlyRepository<Product> ProductsQueries => new ReadOnlyRepository<Product>(
        this.Set<Product>()
    );
}