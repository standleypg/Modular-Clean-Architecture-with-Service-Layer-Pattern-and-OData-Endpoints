using Microsoft.EntityFrameworkCore;
using RetailPortal.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Infrastructure.UnitTests.Data.Repositories;

public class CategoryRepositoryTests: BaseRepositoryTests
{
    private readonly UnitOfWork _uow;

    public CategoryRepositoryTests()
    {
        this._uow = new UnitOfWork(this.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory()
    {
        // Arrange
        var category = (await this.CreateCategory())[0];

        // Act
        var result = await this._uow.CategoryRepository.GetByIdAsync(category.Id, CancellationToken.None);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.CategoryRepository.GetByIdAsync(guid, CancellationToken.None));

        // Assert
        Assert.Equal($"Entity of type {nameof(Category)} with id {guid} not found.", exception.Message);
        Assert.IsType<KeyNotFoundException>(exception);
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = await this.CreateCategory(10);

        // Act
        var result = await this._uow.CategoryRepository.GetAll().ToListAsync();

        // Assert
        Assert.Equal(categories, result);
        Assert.Equal(categories.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCategory()
    {
        // Arrange
        var category = (await this.CreateCategory())[0];

        // Act
        var result = await this._uow.CategoryRepository.AddAsync(category, CancellationToken.None);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task Update_ShouldUpdateCategory()
    {
        // Arrange
        var category = (await this.CreateCategory())[0];
        category.Update("Updated Name");

        // Act
        await this._uow.CategoryRepository.Update(category);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await this._uow.CategoryRepository.GetByIdAsync(category.Id, CancellationToken.None);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task Delete_ShouldDeleteCategory()
    {
        // Arrange
        var category = (await this.CreateCategory())[0];

        // Act
        await this._uow.CategoryRepository.Delete(category);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.CategoryRepository.GetByIdAsync(category.Id, CancellationToken.None));

        // Assert
        Assert.IsType<KeyNotFoundException>(result);
    }

    #region private methods

    private async Task<List<Category>> CreateCategory(int count = 1)
    {
        var categories = new List<Category>();
        await RepositoryUtils.CreateEntity(RepositoryUtils.CreateCategory,async (category, token) =>
        {
            categories.Add(category);
            await this._uow.CategoryRepository.AddAsync(category, token);
            await this._uow.SaveChangesAsync(token);
        }, count);

        return categories;
    }

    #endregion
}