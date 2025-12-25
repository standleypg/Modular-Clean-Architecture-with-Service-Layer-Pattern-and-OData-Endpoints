using Microsoft.AspNetCore.OData.Query;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;

namespace RetailPortal.ServiceFacade.Product;

public interface IProductService
{
    Task<ODataResponse<Model.Db.Entities.Product>> GetAllProduct(GetAllProductRequest request, CancellationToken cancellationToken = default);

    Task<Model.Db.Entities.Product> CreateProduct(CreateProductRequest request,
        CancellationToken cancellationToken = default);
}