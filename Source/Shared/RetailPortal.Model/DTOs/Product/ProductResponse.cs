using RetailPortal.Model.Db.Entities.Common.Enum;

namespace RetailPortal.Model.DTOs.Product;

public record ProductResponse
(
    long ProductId,
    string Name,
    string Description,
    Price Price,
    int Quantity,
    string? ImageUrl,
    ProductCategory Category,
    long? UserId
);

public record Price(decimal Value, string Currency);