using Microsoft.EntityFrameworkCore;
using RetailPortal.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Infrastructure.UnitTests.Data.Repositories;

public class RoleRepositoryTests: BaseRepositoryTests
{
    private readonly UnitOfWork _uow;

    public RoleRepositoryTests()
    {
        this._uow = new UnitOfWork(this.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRole()
    {
        // Arrange
        var role = (await this.CreateRole())[0];

        // Act
        var result = await this._uow.RoleRepository.GetByIdAsync(role.Id, CancellationToken.None);

        // Assert
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.RoleRepository.GetByIdAsync(guid, CancellationToken.None));

        // Assert
        Assert.Equal($"Entity of type {nameof(Role)} with id {guid} not found.", exception.Message);
        Assert.IsType<KeyNotFoundException>(exception);
    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = await this.CreateRole(10);

        // Act
        var result = await this._uow.RoleRepository.GetAll().ToListAsync();

        // Assert
        Assert.Equal(roles, result);
        Assert.Equal(roles.Count, result.Count);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRole()
    {
        // Arrange
        var role = (await this.CreateRole())[0];

        // Act
        var result = await this._uow.RoleRepository.AddAsync(role, CancellationToken.None);

        // Assert
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task Delete_ShouldDeleteRole()
    {
        // Arrange
        var role = (await this.CreateRole())[0];

        // Act
        await this._uow.RoleRepository.Delete(role);
        await this._uow.SaveChangesAsync(CancellationToken.None);
        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            this._uow.RoleRepository.GetByIdAsync(role.Id, CancellationToken.None));

        // Assert
        Assert.IsType<KeyNotFoundException>(result);
    }

    #region private methods

    private async Task<List<Role>> CreateRole(int count = 1)
    {
        var roles = new List<Role>();
        await RepositoryUtils.CreateEntity(RepositoryUtils.CreateRole,async (role, token) =>
        {
            roles.Add(role);
            await this._uow.RoleRepository.AddAsync(role, token);
            await this._uow.SaveChangesAsync(token);
        }, count);

        return roles;
    }

    #endregion
}