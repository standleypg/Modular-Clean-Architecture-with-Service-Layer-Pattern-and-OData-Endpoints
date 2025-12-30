using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RetailPortal.Api.Controllers.Common;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Product;
using RetailPortal.Service.Extensions;
using RetailPortal.ServiceFacade.Product;

namespace RetailPortal.Api.Controllers;

[ApiVersion("0.0")]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[AllowAnonymous] //TODO: Remove AllowAnonymous when auth is implemented
public class ProductController(IProductService productService) : ODataController
{
    // ! This isn't working yet as the we need to provide CategoryId
    // ! Which is not implemented yet
    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var result = await productService.CreateProduct(request);

        return this.Ok(result.Adapt<ProductResponse>());
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProducts()
    {
        var result = await productService.GetAllProduct(queryable => queryable.GetODataResponse<Product, ProductResponse>(this.Request));

        return result.Match(this);
    }
}