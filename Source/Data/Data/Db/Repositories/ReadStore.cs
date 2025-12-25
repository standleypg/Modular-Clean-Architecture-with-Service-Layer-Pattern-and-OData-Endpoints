using RetailPortal.Data.Db.Context;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Repositories;

public class ReadStore(ApplicationDbContext dbContext) : IReadStore
{
    public IReadOnlyRepository<Address> Address => dbContext.AddressesQueries;
    public IReadOnlyRepository<Role> Role => dbContext.RolesQueries;
    public IReadOnlyRepository<User> User => dbContext.UsersQueries;
    public IReadOnlyRepository<Product> Product => dbContext.ProductsQueries;
}