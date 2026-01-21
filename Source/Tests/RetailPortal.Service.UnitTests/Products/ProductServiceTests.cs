using Moq;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Common;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Service.Services.Product;

namespace RetailPortal.Service.UnitTests.Products;

public sealed class ProductServiceTests : IsolatedDatabaseTestBase
{
    private Mock<IReadOnlyRepository<Product>> _mockReadOnlyProductRepository = null!;
    private ProductService _sut = null!;
    private long _testUserId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Mock<IAggregateRepository<Product>> mockProductAggregateRepository = new();
        this._mockReadOnlyProductRepository = new Mock<IReadOnlyRepository<Product>>();

        Mock<IUnitOfWork> mockUow = new();
        mockUow.Setup(u => u.Products).Returns(mockProductAggregateRepository.Object);

        Mock<IReadStore> mockReadStore = new();
        mockReadStore.Setup(r => r.Product).Returns(this._mockReadOnlyProductRepository.Object);

        this._sut = new ProductService(mockUow.Object, mockReadStore.Object);
    }

    private async Task EnsureTestUserExists()
    {
        if (this._testUserId == 0)
        {
            var users = await this.RepositoryUtils.CreateQueryableMockEntities(
                RepositoryUtils.CreateUser,
                uow => uow.Users
            );
            this._testUserId = users.First().Id;
        }
    }

    [Fact]
    public async Task GetAllProduct_ShouldReturnSuccess_WhenProductsExist()
    {
        // Arrange
        var productCount = 10;
        var products = await this.CreateQueryableProductMockEntities(productCount);
        this._mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);

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
        // Arrange
        var productCount = 0;
        var products = await this.CreateQueryableProductMockEntities(productCount);
        this._mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);

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
        var products = await this.CreateQueryableProductMockEntities(5);
        this._mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);
        var expectedErrorMessage = "Test exception message";

        // Act
        var result =
            await this._sut.GetAllProduct<List<Product>>(_ => throw new InvalidOperationException(expectedErrorMessage));

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
        var products = await this.CreateQueryableProductMockEntities(productCount);
        _mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);
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
        var products = await this.CreateQueryableProductMockEntities(productCount);
        this._mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);

        // Act
        var result = await this._sut.GetAllProduct(async queryable => await Task.FromResult(queryable.Take(3).ToList()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task GetAllProduct_ShouldSupportProjectionInExecuteAsync()
    {
        // Arrange
        var productCount = 5;
        var products = await this.CreateQueryableProductMockEntities(productCount);
        this._mockReadOnlyProductRepository.Setup(r => r.GetAll()).Returns(products);

        // Act
        var result = await _sut.GetAllProduct(async queryable =>
            await Task.FromResult(queryable.Select(p => p.Name).ToList()));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(productCount, result.Value.Count);
        Assert.All(result.Value,
            name => Assert.StartsWith("Product", name, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task<IQueryable<Product>> CreateQueryableProductMockEntities(int productCount)
    {
        await this.EnsureTestUserExists();
        return await this.RepositoryUtils.CreateQueryableMockEntities(
            _ => RepositoryUtils.CreateProductWithUser(0, this._testUserId),
            uow => uow.Products,
            productCount
        );
    }
}
