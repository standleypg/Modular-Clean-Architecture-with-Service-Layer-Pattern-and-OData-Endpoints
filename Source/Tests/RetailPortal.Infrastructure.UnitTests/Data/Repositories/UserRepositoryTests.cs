using Microsoft.EntityFrameworkCore;
using RetailPortal.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;

namespace RetailPortal.Infrastructure.UnitTests.Data.Repositories;

public class UserRepositoryTests: BaseRepositoryTests
{
    private readonly UnitOfWork _uow;

    public UserRepositoryTests()
    {
        this._uow = new UnitOfWork(this.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        // Arrange
        var user = (await this.CreateUser())[0];

        // Act
        var result = await this._uow.UserRepository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.UserRepository.GetByIdAsync(guid, CancellationToken.None));

        // Assert
        Assert.Equal($"Entity of type {nameof(User)} with id {guid} not found.", exception.Message);
        Assert.IsType<KeyNotFoundException>(exception);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = await this.CreateUser(10);

        // Act
        var result = await this._uow.UserRepository.GetAll().ToListAsync();

        // Assert
        Assert.Equal(users, result);
        Assert.Equal(users.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        var user = (await this.CreateUser())[0];

        // Act
        var result = await this._uow.UserRepository.AddAsync(user, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task Update_ShouldUpdateUser()
    {
        // Arrange
        var user = (await this.CreateUser())[0];
        var password = Password.Create([1, 2, 3, 4, 5], [1, 2, 3, 4, 5]);
        user.Update("Updated Name", "Updated Lastname", "updated@email.com", password);

        // Act
        await this._uow.UserRepository.Update(user);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await this._uow.UserRepository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task Delete_ShouldDeleteUser()
    {
        // Arrange
        var user = (await this.CreateUser())[0];

        // Act
        await this._uow.UserRepository.Delete(user);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.UserRepository.GetByIdAsync(user.Id, CancellationToken.None));

        // Assert
        Assert.IsType<KeyNotFoundException>(result);
    }

    #region private methods

    private async Task<List<User>> CreateUser(int count = 1)
    {
        var users = new List<User>();
        await RepositoryUtils.CreateEntity(RepositoryUtils.CreateUser,async (user, token) =>
        {
            users.Add(user);
            await this._uow.UserRepository.AddAsync(user, token);
            await this._uow.SaveChangesAsync(token);
        }, count);

        return users;
    }

    #endregion
}