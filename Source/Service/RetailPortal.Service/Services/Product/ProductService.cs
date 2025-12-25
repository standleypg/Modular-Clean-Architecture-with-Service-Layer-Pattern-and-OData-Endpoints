using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OData.Query;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;
using RetailPortal.Service.Extensions;
using RetailPortal.ServiceFacade.Product;
using Price = RetailPortal.Model.Db.Entities.Common.ValueObjects.Price;

namespace RetailPortal.Service.Services.Product;

public class ProductService(IUnitOfWork uow, IReadStore readStore, IMapper mapper): IProductService
{
    public async Task<ODataResponse<Model.Db.Entities.Product>> GetAllProduct(GetAllProductRequest request, CancellationToken cancellationToken)
    {
        var options = request.options;

        ArgumentNullException.ThrowIfNull(options);

        var products = readStore.Product.GetAll();

        var oDataResponse = await products.GetODataResponseAsync(options, cancellationToken);

        return mapper.Map<ODataResponse<Model.Db.Entities.Product>>(oDataResponse);
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