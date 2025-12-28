using Microsoft.EntityFrameworkCore.Storage;
using RetailPortal.Data.Db.Context;
using RetailPortal.Data.Db.Repositories;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Db.Entities;
using System.Diagnostics.CodeAnalysis;

namespace RetailPortal.Data.Db.UnitOfWork;

public sealed class UnitOfWork(ApplicationDbContext context)
    : IUnitOfWork, IAsyncDisposable
{
    private bool _disposed;

    private IDbContextTransaction? _currentTransaction;
    public IAggregateRepository<User> Users => context.Users;
    public IAggregateRepository<Product> Products { get; } = context.Products;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Attach<TEntity>(TEntity entity) where TEntity : class
    {
        context.Attach(entity);
    }

    public async Task BeginTransactionAsync()
    {
        if (this._currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        this._currentTransaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (this._currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await context.SaveChangesAsync();
            await this._currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (this._currentTransaction != null)
            {
                await this._currentTransaction.DisposeAsync();
                this._currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (this._currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await this._currentTransaction.RollbackAsync();
        }
        finally
        {
            if (this._currentTransaction != null)
            {
                await this._currentTransaction.DisposeAsync();
                this._currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposed && disposing)
        {
            this._currentTransaction?.Dispose();
            context.Dispose();
        }
        this._disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (this._currentTransaction != null)
        {
            await this._currentTransaction.DisposeAsync();
        }

        await context.DisposeAsync();
    }
}