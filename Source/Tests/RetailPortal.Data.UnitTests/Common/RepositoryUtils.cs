using RetailPortal.Data.Db.Context;
using RetailPortal.Data.Db.UnitOfWork;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.Enum;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Infrastructure.UnitTests.Common;

public class RepositoryUtils(ApplicationDbContext context) : IDisposable
{
    private readonly UnitOfWork _unitOfWork = new(context);

    public async Task<IQueryable<TEntity>> CreateQueryableMockEntities<TEntity>(
        Func<int, TEntity> createEntity,
        Func<IUnitOfWork, IAggregateRepository<TEntity>> resolveRepository,
        int count = 1,
        CancellationToken cancellationToken = default) where TEntity : EntityBase
    {
        var repository = resolveRepository(this._unitOfWork);

        for (int i = 0; i < count; i++)
        {
            var entity = createEntity(i);
            repository.Add(entity);
        }

        await this._unitOfWork.SaveChangesAsync(cancellationToken);

        return repository.GetAll();
    }

    public async Task<IQueryable<TEntity>> CreateQueryableMockEntities<TEntity>(
        List<TEntity> entities,
        Func<IUnitOfWork, IAggregateRepository<TEntity>> resolveRepository,
        CancellationToken cancellationToken = default) where TEntity : EntityBase
    {
        var repository = resolveRepository(this._unitOfWork);

        foreach (var entity in entities)
        {
            repository.Add(entity);
        }

        await this._unitOfWork.SaveChangesAsync(cancellationToken);

        return repository.GetAll();
    }

    public static async Task CreateEntity<T>(IUnitOfWork uow, Func<int, T> createEntity,
        Func<T, CancellationToken, Task>? execute, int count = 1,
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
        var values = Enum.GetValues<ProductCategory>();
        var random = new Random();
        var randomCategory = (ProductCategory)values.GetValue(random.Next(values.Length))!;
        product.AddCategory(randomCategory);
        return product;
    }

    public static Product CreateProductWithUser(int i, long userId)
    {
        var product = Product.Create($"Product {i}", $"Description {i}", Price.Create(i, "MYR"), i, null);
        var values = Enum.GetValues<ProductCategory>();
        var random = new Random();
        var randomCategory = (ProductCategory)values.GetValue(random.Next(values.Length))!;
        product.AddCategory(randomCategory);
        product.UserId = userId;
        return product;
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

    public void Dispose()
    {
        this._unitOfWork.Dispose();
    }
}