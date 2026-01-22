using MockQueryable;
using Moq;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Infrastructure.UnitTests.Common;

public abstract class ServiceTestBase : IAsyncLifetime
{
    protected RepositoryUtils RepositoryUtils { get; } = new();

    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected static (Mock<IUnitOfWork>, Mock<IReadStore>) CreateUowAndReadStoreMocks()
    {
        var usersMock = new List<User>().BuildMock;
        var productsMock = new List<Product>().BuildMock;
        var rolesMock = new List<Role>().BuildMock;

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        uowMock.Setup(u => u.BeginTransactionAsync());
        uowMock.Setup(u => u.RollbackTransactionAsync());
        uowMock.Setup(u => u.CommitTransactionAsync());
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));
        uowMock.Setup(u => u.Products.GetAll())
            .Returns(productsMock);
        uowMock.Setup(u => u.Users.GetAll())
            .Returns(usersMock);
        uowMock.Setup(u => u.Roles.GetAll())
            .Returns(rolesMock);

        var readStoreMock = new Mock<IReadStore>();
        readStoreMock.Setup(r => r.Product.GetAll())
            .Returns(productsMock);
        readStoreMock.Setup(r => r.User.GetAll())
            .Returns(usersMock);
        readStoreMock.Setup(r => r.Role.GetAll())
            .Returns(rolesMock);

        return (uowMock, readStoreMock);
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
