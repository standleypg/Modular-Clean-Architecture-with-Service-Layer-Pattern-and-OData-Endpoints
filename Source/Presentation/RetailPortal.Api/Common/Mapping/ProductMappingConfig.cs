using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;

namespace RetailPortal.Api.Common.Mapping;

public class ProductMappingConfig : Profile
{
    public ProductMappingConfig()
    {
        this.CreateMap<CreateProductRequest, CreateProductRequest>();

           this.CreateMap<Product, ProductResponse>()
               .ConstructUsing(product => new ProductResponse(product.Id, product.Name, product.Description, new Price(product.Price.Value, product.Price.Currency), product.Quantity, product.ImageUrl, product.Category, product.User.Id));

           this.CreateMap<ODataQueryOptions<Product>, GetAllProductRequest>()
               .ConstructUsing(options => new GetAllProductRequest(options));

           this.CreateMap<Model.Db.Entities.Common.ValueObjects.Price, Price>()
               .ConstructUsing(price => new Price(price.Value, price.Currency));

           this.CreateMap<ODataResponse<Product>, ODataResponse<ProductResponse>>()
               .ConstructUsing(products => new ODataResponse<ProductResponse>()
               {
                     Count = products.Count,
                     NextPage = products.NextPage,
                     Value = (products.Value ?? new List<Product>())
                         .Select(product => new ProductResponse(
                             product.Id,
                             product.Name,
                             product.Description,
                             new Price(product.Price.Value, product.Price.Currency),
                             product.Quantity,
                             product.ImageUrl,
                             product.Category,
                             product.UserId
                         )).ToList()
               });
    }
}