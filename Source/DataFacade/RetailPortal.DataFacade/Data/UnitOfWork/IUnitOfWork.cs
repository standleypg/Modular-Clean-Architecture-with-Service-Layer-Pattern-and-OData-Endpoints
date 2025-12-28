using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.DataFacade.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IAggregateRepository<User> Users { get; }
    IAggregateRepository<Product> Products { get; }
    /// <summary>
    /// Saves all changes to the database in a single transaction.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins an explicit transaction for coordinating multiple operations.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction, saving all changes to the database.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction, discarding all changes.
    /// </summary>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Attaches an entity to the context in the Unchanged state,
    /// indicating it already exists in the database.
    /// </summary>
    void Attach<TEntity>(TEntity entity) where TEntity : class;
}