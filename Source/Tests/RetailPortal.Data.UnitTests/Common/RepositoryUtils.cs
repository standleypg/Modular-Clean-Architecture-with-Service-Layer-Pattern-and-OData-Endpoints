using MockQueryable;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.Enum;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Infrastructure.UnitTests.Common;

public class RepositoryUtils
{
    public IQueryable<TEntity> MockEntities<TEntity>(int count = 1) where TEntity : EntityBase
    {
        var createEntity = this.GetCreateEntityPredicate<TEntity>();
        var entities = new List<TEntity>();
        for (int i = 0; i < count; i++)
        {
            var entity = createEntity(i);
            entities.Add(entity);
        }

        return entities.BuildMock();
    }

    public IQueryable<TEntity> MockEntitiesFromEnum<TEnum, TEntity>()
        where TEnum : struct, Enum
        where TEntity : EntityBase
    {
        var createFunc = GetCreateEntityFromEnumPredicate<TEnum, TEntity>();

        return Enum.GetValues<TEnum>()
            .Select(createFunc)
            .ToList().BuildMock();
    }
    private static Product CreateProduct(int i)
    {
        var product = Product.Create($"Product {i}", $"Description {i}", Price.Create(i, "MYR"), i, null);
        var values = Enum.GetValues<ProductCategory>();
        var random = new Random();
        var randomCategory = (ProductCategory)values.GetValue(random.Next(values.Length))!;
        product.AddCategory(randomCategory);
        return product;
    }

    private static Role CreateRole(int i)
    {
        var role = Role.Create($"Role {i}", $"Description {i}");
        return role;
    }

    private static User CreateUser(int i)
    {
        var password = Password.Create([1, 2, 3, 4, 5, 6, 7, 8, 9, 0], [1, 2, 3, 4, 5, 6, 7, 8, 9, 0]);
        var user = User.Create($"Firstname {i}", $"Lastname {i}", $"{i}@email.com", password: password);
        return user;
    }

    private Func<int, TEntity> GetCreateEntityPredicate<TEntity>() where TEntity : EntityBase
    {
        return typeof(TEntity) switch
        {
            var t when t == typeof(Product) => i => (TEntity)(object)CreateProduct(i),
            var t when t == typeof(Role) => i => (TEntity)(object)CreateRole(i),
            var t when t == typeof(User) => i => (TEntity)(object)CreateUser(i),
            _ => throw new NotImplementedException($"No predicate defined for type {typeof(TEntity).Name}"),
        };
    }

    private static Func<TEnum, TEntity> GetCreateEntityFromEnumPredicate<TEnum, TEntity>()
        where TEnum : struct, Enum
        where TEntity : EntityBase
    {
        return (typeof(TEntity), typeof(TEnum)) switch
        {
            var (entity, @enum) when entity == typeof(Role) && @enum == typeof(Roles)
                => enumValue => (TEntity)(object)Role.Create(enumValue.ToString(), $"{enumValue} role"),
            _ => throw new NotImplementedException(
                $"No predicate defined for Entity: {typeof(TEntity).Name}, Enum: {typeof(TEnum).Name}")
        };
    }
}