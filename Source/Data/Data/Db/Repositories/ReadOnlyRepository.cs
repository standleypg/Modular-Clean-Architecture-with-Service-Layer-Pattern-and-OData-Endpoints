using Microsoft.EntityFrameworkCore;
using RetailPortal.DataFacade.Data.Repositories;

namespace RetailPortal.Data.Db.Repositories;

public class ReadOnlyRepository<T>(IQueryable<T> queryable) : IReadOnlyRepository<T>
    where T : class
{
    public IQueryable<T> GetAll() => queryable.AsNoTracking();
}