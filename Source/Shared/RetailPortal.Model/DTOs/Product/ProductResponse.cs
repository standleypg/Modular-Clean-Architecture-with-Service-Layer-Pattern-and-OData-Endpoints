using RetailPortal.Model.Db.Entities.Common.Enum;

namespace RetailPortal.Model.DTOs.Product;

public record ProductResponse
(
    ulong ProductId,
    string Name,
    string Description,
    Price Price,
    int Quantity,
    string? ImageUrl,
    ProductCategory Category,
    ulong? UserId
);

public record Price(decimal Value, string Currency);