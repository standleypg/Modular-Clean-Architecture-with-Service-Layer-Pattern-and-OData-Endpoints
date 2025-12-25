using Microsoft.AspNetCore.OData.Query;

namespace RetailPortal.Model.DTOs.Product;

public record GetAllProductRequest(ODataQueryOptions<Db.Entities.Product> options);