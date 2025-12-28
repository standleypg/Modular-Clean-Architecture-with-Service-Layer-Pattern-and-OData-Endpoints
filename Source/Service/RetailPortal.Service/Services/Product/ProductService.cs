using Mapster;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;
using RetailPortal.ServiceFacade.Product;
using Price = RetailPortal.Model.Db.Entities.Common.ValueObjects.Price;

namespace RetailPortal.Service.Services.Product;

public class ProductService(IUnitOfWork uow, IReadStore readStore): IProductService
{
    public Task<TResult> GetAllProduct<TResult>(Func<IQueryable<ProductResponse>, Task<TResult>> executeAsync)
    {
        var products = readStore.Product.GetAll();

        return executeAsync(products.ProjectToType<ProductResponse>());
    }

    public async Task<Model.Db.Entities.Product> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var price = Price.Create(request.Price.Value, request.Price.Currency);

        var product = Model.Db.Entities.Product.Create(request.Name, request.Description, price, request.Quantity, null);

        uow.Products.Add(product);
        await uow.SaveChangesAsync(cancellationToken);

        return product;
    }
}