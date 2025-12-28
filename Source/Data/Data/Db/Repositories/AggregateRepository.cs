using Microsoft.EntityFrameworkCore;
using RetailPortal.DataFacade.Data.Repositories;
using System.Linq.Expressions;

namespace RetailPortal.Data.Db.Repositories;

public class AggregateRepository<T>(DbSet<T> dbSet, IQueryable<T>? aggregateQuery = null)
    : IAggregateRepository<T>
    where T : class
{
    public IQueryable<T> GetAll() => aggregateQuery ?? dbSet;

    public void Add(T entity) =>
        dbSet.Add(entity);

    public void Remove(T entity) =>
        dbSet.Remove(entity);
}