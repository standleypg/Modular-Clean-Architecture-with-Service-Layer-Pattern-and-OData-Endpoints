using RetailPortal.Model.Db.Entities;

namespace RetailPortal.DataFacade.Data.Repositories;

public interface IReadStore
{
    IReadOnlyRepository<Address> Address { get; }
    IReadOnlyRepository<Role> Role { get; }
    IReadOnlyRepository<Product> Product { get; }
    IReadOnlyRepository<User> User { get; }
}