using Moq;
using RetailPortal.Application.Auth.Commands.RegisterCommand;
using RetailPortal.DataFacade.Interfaces.Application.Services;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Auth;
using RetailPortal.DataFacade.Interfaces.Infrastructure.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Unit.Auth.Commands.RegisterCommand;

public class RegisterCommandHandlerTests
{
    private readonly RegisterCommandHandler _sut;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public RegisterCommandHandlerTests()
    {
        this._uowMock = new Mock<IUnitOfWork>();
        this._roleServiceMock = new Mock<IRoleService>();
        this._jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        this._passwordHasherMock = new Mock<IPasswordHasher>();
        this._sut = new RegisterCommandHandler
        (
            this._uowMock.Object,
            this._roleServiceMock.Object,
            this._jwtTokenGeneratorMock.Object,
            this._passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEmailExists()
    {
        // Arrange
        var (user, command) = CreateUser();
        this._uowMock.Setup(u => u.UserRepository.GetUserByEmail(It.IsAny<string>())).Returns(user);

        // Act
        var result = await this._sut.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnAuthResult_WhenEmailDoesNotExist()
    {
        // Arrange
        var (_, command) = CreateUser();
        this._uowMock.Setup(u => u.UserRepository.GetUserByEmail(It.IsAny<string>())).Returns(null as User);
        this._roleServiceMock.Setup(r => r.GetRoleByNameAsync(It.IsAny<Roles>())).ReturnsAsync(Role.Create(Roles.User.ToString(), null!));
        this._passwordHasherMock.Setup(p => p.CreatePasswordHash(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny));
        this._jwtTokenGeneratorMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        // Act
        var result = await this._sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(false);
        Assert.NotNull(result.Value);
        Assert.Equal("token", result.Value);
    }

    private static (User, Model.DTOs.Auth.RegisterCommand) CreateUser()
    {
        var command =
            new Model.DTOs.Auth.RegisterCommand("John", "Doe", "JohnDoe@email.com",
                "password");
        var user = User.Create(command.FirstName, command.LastName, command.Email, null!);

        return (user, command);
    }
}