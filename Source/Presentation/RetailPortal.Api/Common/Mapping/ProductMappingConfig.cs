using Mapster;
using Microsoft.AspNetCore.OData.Query;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;

namespace RetailPortal.Api.Common.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductRequest, CreateProductRequest>();

        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.ProductId, src => src.Id);

        config.NewConfig<Model.Db.Entities.Common.ValueObjects.Price, Price>()
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Currency, src => src.Currency);

        config.ForType(typeof(ODataResponse<>), typeof(ODataResponse<>))
            .Map(nameof(ODataResponse<>.Count), nameof(ODataResponse<>.Count))
            .Map(nameof(ODataResponse<>.NextPage), nameof(ODataResponse<>.NextPage))
            .Map(nameof(ODataResponse<>.Value), nameof(ODataResponse<>.Value));
    }
}
