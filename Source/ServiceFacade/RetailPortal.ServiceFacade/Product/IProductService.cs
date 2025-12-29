using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;

namespace RetailPortal.ServiceFacade.Product;

public interface IProductService
{
    Task<Result<TResult, string>> GetAllProduct<TResult>(Func<IQueryable<Model.Db.Entities.Product>, Task<TResult>> executeAsync);

    Task<Model.Db.Entities.Product> CreateProduct(CreateProductRequest request,
        CancellationToken cancellationToken = default);
}