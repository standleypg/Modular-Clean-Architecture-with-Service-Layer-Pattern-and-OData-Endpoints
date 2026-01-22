using MockQueryable;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.Enum;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Infrastructure.UnitTests.Common;

public class RepositoryUtils
{
    public IQueryable<TEntity> CreateQueryableMockEntities<TEntity>(
        Func<int, TEntity> createEntity, int count = 1) where TEntity : EntityBase
    {
        var entities = new List<TEntity>();
        for (int i = 0; i < count; i++)
        {
            var entity = createEntity(i);
            entities.Add(entity);
        }

        var mockQueryable = entities.BuildMock();
        return mockQueryable;
    }

    public IQueryable<TEntity> CreateQueryableMockEntities<TEntity>(
        List<TEntity> entities) where TEntity : EntityBase
    {
        var mockQueryable = entities.BuildMock();
        return mockQueryable;
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