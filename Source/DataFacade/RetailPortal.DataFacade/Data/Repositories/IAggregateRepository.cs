namespace RetailPortal.DataFacade.Data.Repositories;

public interface IAggregateRepository<T>
{
    IQueryable<T> GetAll();
    void Add(T entity);
    void Remove(T entity);
}