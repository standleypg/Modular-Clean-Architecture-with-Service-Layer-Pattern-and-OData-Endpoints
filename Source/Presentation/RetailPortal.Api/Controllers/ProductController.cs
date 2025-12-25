using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RetailPortal.Api.Common.Http;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Model.DTOs.Product;
using RetailPortal.ServiceFacade.Product;

namespace RetailPortal.Api.Controllers;

[ApiVersion("0.0")]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[AllowAnonymous] //TODO: Remove AllowAnonymous when auth is implemented
public class ProductController(IMapper mapper, IProductService productService) : ODataController
{
    // ! This isn't working yet as the we need to provide CategoryId
    // ! Which is not implemented yet
    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var result = await productService.CreateProduct(request);

        return this.Ok(mapper.Map<ProductResponse>(result));
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProducts()
    {
        var options = this.Request.GetODataQueryOptions<Product>();
        var result = await productService.GetAllProduct(mapper.Map<GetAllProductRequest>(options));

        //TODO: map this mapping using mapper
        return this.Ok(new ODataResponse<ProductResponse>()
        {
            Count = result.Count,
            NextPage = result.NextPage,
            Value = (result.Value ?? new List<Product>())
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