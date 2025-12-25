using Moq;
using RetailPortal.Application.Products.Queries.GetAllProduct;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Data.Repositories;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Data.UnitOfWork;
using RetailPortal.Domain.Interfaces.Infrastructure.Data.Repositories;
using RetailPortal.Domain.Interfaces.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Unit.Products.Queries.GetAllProduct;

public sealed class GetAllProductHandlerTests : IDisposable
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly GetAllProductHandler _sut;
    private readonly RepositoryUtils _repositoryUtils;

    public GetAllProductHandlerTests()
    {
        Mock<IProductRepository> mockProductRepository = new();
        this._repositoryUtils = new RepositoryUtils();
        this._mockUow = new Mock<IUnitOfWork>();
        this._mockUow.Setup(u => u.ProductRepository).Returns(mockProductRepository.Object);
        this._sut = new GetAllProductHandler(this._mockUow.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnODataResponse()
    {
        // Arrange
        var productCount = 10;
        var queryOptions = TestUtils.ODataQueryOptionsUtils<Product>(productCount);
        var products = await this.CreateQueryableProductMockEntities(productCount);

        // Act
        this._mockUow.Setup(r => r.ProductRepository.GetAll()).Returns(products);
        var result = await this._sut.Handle(new GetAllProductCommand(queryOptions), It.IsAny<CancellationToken>());

        // Assert
        var response = result.Value;
        Assert.Equal(productCount
            , response.Count);
        Assert.Equal(products, response.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnODataResponseWithNoProducts()
    {
        // Arrange
        var productCount = 0;
        var queryOptions = TestUtils.ODataQueryOptionsUtils<Product>(productCount);
        var products = await this.CreateQueryableProductMockEntities(productCount);

        // Act
        this._mockUow.Setup(r => r.ProductRepository.GetAll()).Returns(products);
        var result = await this._sut.Handle(new GetAllProductCommand(queryOptions), It.IsAny<CancellationToken>());

        // Assert
        var response = result.Value;
        Assert.Equal(productCount
            , response.Count);
        Assert.Equal(products, response.Value);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSelectQueryIsIncluded()
    {
        // Arrange
        var productCount = 10;
        var queryOptions = TestUtils.ODataQueryOptionsUtils<Product>(productCount, includeSelectQuery: true);
        var products = await this.CreateQueryableProductMockEntities(productCount);

        // Act
        this._mockUow.Setup(r => r.ProductRepository.GetAll()).Returns(products);
        var result = await this._sut.Handle(new GetAllProductCommand(queryOptions), It.IsAny<CancellationToken>());

        // Assert
        Assert.True(result.IsError);
    }

    private Task<IQueryable<Product>> CreateQueryableProductMockEntities(int productCount)
    {
        return this._repositoryUtils.CreateQueryableMockEntities(
            RepositoryUtils.CreateProduct,
            uow => uow.ProductRepository,
            productCount
        );
    }

    public void Dispose()
    {
        this._repositoryUtils.Dispose();
        GC.SuppressFinalize(this);
    }

    ~GetAllProductHandlerTests()
    {
        this.Dispose();
    }
}