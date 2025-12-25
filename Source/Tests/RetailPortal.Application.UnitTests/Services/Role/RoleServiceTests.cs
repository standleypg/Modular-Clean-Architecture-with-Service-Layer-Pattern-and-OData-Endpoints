using Microsoft.EntityFrameworkCore;
using Moq;
using RetailPortal.Application.Services.Role;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Data.Repositories;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Constants;

namespace RetailPortal.Unit.Services.Role;

public sealed class RoleServiceTests : IDisposable
{
    private readonly RoleService _sut;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly RepositoryUtils _repositoryUtils;

    public RoleServiceTests()
    {
        this._repositoryUtils = new RepositoryUtils();
        this._uowMock = new Mock<IUnitOfWork>();
        this._uowMock.Setup(u => u.RoleRepository).Returns(Mock.Of<IRoleRepository>());
        this._sut = new RoleService(this._uowMock.Object);
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnListOfRoles()
    {
        // Arrange
        var roles = await this.CreateQueryableRoleMockEntities();
        this._uowMock.Setup(u => u.RoleRepository.GetAll()).Returns(roles);

        // Act
        var result = await this._sut.GetAllRolesAsync();

        // Assert
        Assert.Equal((await roles.ToListAsync()).Count, result.Count);
    }

    [Fact]
    public async Task GetRoleByNameAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var roles = await this.CreateQueryableRoleMockEntities();
        this._uowMock.Setup(u => u.RoleRepository.GetAll()).Returns(roles);

        // Act
        var result = await this._sut.GetRoleByNameAsync(Roles.Admin);

        // Assert
        Assert.Equal((await roles.ToListAsync())[0].Name, result.Name);
    }

    [Fact]
    public async Task GetRoleByNameAsync_ShouldThrowException_WhenRoleDoesNotExist()
    {
        // Arrange
        var roles = await this.CreateQueryableRoleMockEntities();
        this._uowMock.Setup(u => u.RoleRepository.GetAll()).Returns(roles);

        // Act
        Func<Task> act = async () => await this._sut.GetRoleByNameAsync(Roles.Seller);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    private Task<IQueryable<Model.Db.Entities.Role>> CreateQueryableRoleMockEntities()
    {
        var roles = (from role in Enum.GetValues<Roles>()
            where role != Roles.Seller
            select Model.Db.Entities.Role.Create(role.ToString(), $"{role} role")).ToList();
        return this._repositoryUtils.CreateQueryableMockEntities(
            roles,
            uow => uow.RoleRepository
        );
    }

    public void Dispose()
    {
        this._repositoryUtils.Dispose();
        GC.SuppressFinalize(this);
    }

    ~RoleServiceTests()
    {
        this.Dispose();
    }
}