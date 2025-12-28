using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;

namespace RetailPortal.ServiceFacade.Product;

public interface IProductService
{
    Task<TResult> GetAllProduct<TResult>(Func<IQueryable<ProductResponse>, Task<TResult>> executeAsync);

    Task<Model.Db.Entities.Product> CreateProduct(CreateProductRequest request,
        CancellationToken cancellationToken = default);
}