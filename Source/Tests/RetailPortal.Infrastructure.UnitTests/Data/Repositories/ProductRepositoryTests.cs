using Microsoft.EntityFrameworkCore;
using RetailPortal.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Infrastructure.UnitTests.Data.Repositories;

public class ProductRepositoryTests : BaseRepositoryTests
{
    private readonly UnitOfWork _uow;

    public ProductRepositoryTests()
    {
        this._uow = new UnitOfWork(this.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var product = (await this.CreateProduct())[0];

        // Act
        var result = await this._uow.ProductRepository.GetByIdAsync(product.Id, CancellationToken.None);

        // Assert
        Assert.Equal(product, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.ProductRepository.GetByIdAsync(guid, CancellationToken.None));

        // Assert
        Assert.Equal($"Entity of type {nameof(Product)} with id {guid} not found.", exception.Message);
        Assert.IsType<KeyNotFoundException>(exception);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var products = await this.CreateProduct(10);

        // Act
        var result = await this._uow.ProductRepository.GetAll().ToListAsync();

        // Assert
        Assert.Equal(products, result);
        Assert.Equal(products.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = (await this.CreateProduct())[0];

        // Act
        var result = await this._uow.ProductRepository.AddAsync(product, CancellationToken.None);

        // Assert
        Assert.Equal(product, result);
    }

    [Fact]
    public async Task Update_ShouldUpdateProduct()
    {
        // Arrange
        var product = (await this.CreateProduct())[0];
        product.Update("Updated Name", "Updated Description");

        // Act
        await this._uow.ProductRepository.Update(product);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await this._uow.ProductRepository.GetByIdAsync(product.Id, CancellationToken.None);

        // Assert
        Assert.Equal(product, result);
    }

    [Fact]
    public async Task Delete_ShouldDeleteProduct()
    {
        // Arrange
        var product = (await this.CreateProduct())[0];

        // Act
        await this._uow.ProductRepository.Delete(product);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.ProductRepository.GetByIdAsync(product.Id, CancellationToken.None));

        // Assert
        Assert.IsType<KeyNotFoundException>(result);
    }

    #region private methods

    private async Task<List<Product>> CreateProduct(int count = 1)
    {
        var products = new List<Product>();
        await RepositoryUtils.CreateEntity(RepositoryUtils.CreateProduct, async (product, token) =>
        {
            products.Add(product);
            await this._uow.ProductRepository.AddAsync(product, token);
            await this._uow.SaveChangesAsync(token);
        }, count);

        return products;
    }

    #endregion
}