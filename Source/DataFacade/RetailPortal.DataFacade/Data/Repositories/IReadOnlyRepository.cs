namespace RetailPortal.DataFacade.Data.Repositories;

public interface IReadOnlyRepository<out T> where T : class
{
    IQueryable<T> GetAll();
}