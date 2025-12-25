using RetailPortal.Domain.Interfaces.Infrastructure.Data.Repositories;
using RetailPortal.Domain.Interfaces.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.Data.UnitOfWork;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;

public class RepositoryUtils : BaseRepositoryTests
{
    private readonly UnitOfWork _uow;

    public RepositoryUtils()
    {
        this._uow = new UnitOfWork(Context);
    }

    public async Task<IQueryable<TEntity>> CreateQueryableMockEntities<TEntity>(
        Func<int, TEntity> createEntity,
        Func<IUnitOfWork, IGenericRepository<TEntity>> resolveRepository,
        int count = 1,
        CancellationToken cancellationToken = default) where TEntity : EntityBase
    {
        var repository = resolveRepository(this._uow);

        for (int i = 0; i < count; i++)
        {
            var entity = createEntity(i);
            await repository.AddAsync(entity, cancellationToken);
        }

        await this._uow.SaveChangesAsync(cancellationToken);

        return repository.GetAll();
    }

    public async Task<IQueryable<TEntity>> CreateQueryableMockEntities<TEntity>(
        List<TEntity> entities,
        Func<IUnitOfWork, IGenericRepository<TEntity>> resolveRepository,
        CancellationToken cancellationToken = default) where TEntity : EntityBase
    {
        var repository = resolveRepository(this._uow);

        foreach (var entity in entities)
        {
            await repository.AddAsync(entity, cancellationToken);
        }

        await this._uow.SaveChangesAsync(cancellationToken);

        return repository.GetAll();
    }

    public static async Task CreateEntity<T>(Func<int, T> createEntity, Func<T, CancellationToken, Task>? execute, int count = 1,
        CancellationToken cancellationToken = default)
    {
        for (var i = 1; i <= count; i++)
        {
            var entity = createEntity(i);
            if (execute != null)
            {
                await execute(entity, cancellationToken);
            }
        }
    }

    public static Product CreateProduct(int i)
    {
        var product = Product.Create($"Product {i}", $"Description {i}", Price.Create(i, "MYR"), i, null);
        product.AddCategory(Guid.NewGuid());
        product.AddSeller(Guid.NewGuid());
        return product;
    }

    public static Category CreateCategory(int i)
    {
        var category = Category.Create($"Category {i}");
        return category;
    }

    public static Role CreateRole(int i)
    {
        var role = Role.Create($"Role {i}", $"Description {i}");
        return role;
    }

    public static User CreateUser(int i)
    {
        var password = Password.Create([1, 2, 3, 4, 5, 6, 7, 8, 9, 0], [1, 2, 3, 4, 5, 6, 7, 8, 9, 0]);
        var user = User.Create($"Firstname {i}", $"Lastname {i}", $"{i}@email.com", password: password);
        return user;
    }
}