using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.Enum;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Model.Db.Entities;

public sealed class Product: EntityBase
{
    public string Name { get; private set;}
    public string Description { get; private set;}
    public Price Price { get; private set;}
    public int Quantity { get; private set;}
    public string? ImageUrl { get; private set;}
    public ProductCategory Category { get; private set; } = ProductCategory.None;
    public ulong UserId { get; set; }
    public User User { get; private set;} = default!;

    // Empty constructor for EF Core required when we have Value Objects in the entity
    private Product() { }

    private Product(string name, string description, Price price, int quantity, string? imageUrl)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Quantity = quantity;
        this.ImageUrl = imageUrl;
    }

    public static Product Create(string name, string description, Price price, int quantity, string? imageUrl)
    {
        return new Product(name, description, price, quantity, imageUrl);
    }

    public void Update(string? name = null, string? description = null, Price? price = null, int? quantity = null, string? imageUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            this.Name = name;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            this.Description = description;
        }

        if (price is not null)
        {
            this.Price = price;
        }

        if (quantity is not null)
        {
            this.Quantity = quantity.Value;
        }

        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            this.ImageUrl = imageUrl;
        }
    }

    public void AddCategory(ProductCategory category)
    {
        this.Category = category;
    }
}