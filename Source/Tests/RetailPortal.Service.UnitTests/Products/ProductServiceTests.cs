using Moq;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Common;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Service.Services.Product;

namespace RetailPortal.Service.UnitTests.Products;

public sealed class ProductServiceTests : ServiceTestBase
{
    private readonly Mock<IReadStore> _readStoreMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        (Mock<IUnitOfWork> uowMock, this._readStoreMock) = CreateUowAndReadStoreMocks();
        this._sut = new ProductService(uowMock.Object, this._readStoreMock.Object);
    }

    [Fact]
    public async Task GetAllProduct_ShouldReturnSuccess_WhenProductsExist()
    {
        // Arrange
        var productCount = 10;
        this._readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(this.CreateProductEntities(productCount));

        // Act
        var result = await this._sut.GetAllProduct(async queryable =>
        {
            var list = await Task.FromResult(queryable.ToList());
            return list;
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(productCount, result.Value.Count);
    }

    [Fact]
    public async Task GetAllProduct_ShouldReturnSuccess_WhenNoProductsExist()
    {
        // Arrange - no products created

        // Act
        var result = await this._sut.GetAllProduct(async queryable => await Task.FromResult(queryable.ToList()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllProduct_ShouldReturnFailure_WhenExecuteAsyncThrowsException()
    {
        // Arrange
        this._readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(this.CreateProductEntities(5));
        var expectedErrorMessage = "Test exception message";

        // Act
        var result =
            await this._sut.GetAllProduct<List<Product>>(_ =>
                throw new InvalidOperationException(expectedErrorMessage));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedErrorMessage, result.Errors[Result<List<Product>, string>.DefaultErrorKey].First(),
            StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public async Task GetAllProduct_ShouldPassCorrectQueryableToExecuteAsync()
    {
        // Arrange
        var productCount = 5;
        this._readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(this.CreateProductEntities(productCount));
        IQueryable<Product>? capturedQueryable = null;

        // Act
        var result = await this._sut.GetAllProduct(async queryable =>
        {
            capturedQueryable = queryable;
            return await Task.FromResult(queryable.Count());
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedQueryable);
        Assert.Equal(productCount, result.Value);
    }

    [Fact]
    public async Task GetAllProduct_ShouldSupportFilteringInExecuteAsync()
    {
        // Arrange
        var productCount = 10;
        this._readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(this.CreateProductEntities(productCount));

        // Act
        var result =
            await this._sut.GetAllProduct(async queryable => await Task.FromResult(queryable.Take(3).ToList()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task GetAllProduct_ShouldSupportProjectionInExecuteAsync()
    {
        // Arrange
        var productCount = 5;
        this._readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(this.CreateProductEntities(productCount));

        // Act
        var result = await this._sut.GetAllProduct(async queryable =>
            await Task.FromResult(queryable.Select(p => p.Name).ToList()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(productCount, result.Value.Count);
        Assert.All(result.Value,
            name => Assert.StartsWith("Product", name, StringComparison.InvariantCultureIgnoreCase));
    }

    private IQueryable<Product> CreateProductEntities(int productCount)
    {
        this.RepositoryUtils.MockEntities<User>();

        return this.RepositoryUtils.MockEntities<Product>(productCount);
    }
}