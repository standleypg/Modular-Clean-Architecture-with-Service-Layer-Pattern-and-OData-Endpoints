using Moq;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Infrastructure.UnitTests.Common;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.Service.Services.Auth;
using RetailPortal.ServiceFacade.Validator.Common;

namespace RetailPortal.Service.UnitTests.Auth;

public sealed class RegisterServiceTests : ServiceTestBase
{
    private readonly RegisterService _sut;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IValidator> _mockValidator;

    public RegisterServiceTests()
    {
        (this._uowMock, _) = CreateUowAndReadStoreMocks();
        this._jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        this._passwordHasherMock = new Mock<IPasswordHasher>();
        this._mockValidator = new Mock<IValidator>();
        this._sut = new RegisterService
        (
            this._uowMock.Object,
            this._jwtTokenGeneratorMock.Object,
            this._passwordHasherMock.Object,
            this._mockValidator.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEmailExists()
    {
        // Arrange
        var (user, request) = CreateUser();
        this._uowMock.Setup(u => u.Users.GetAll()).Returns(user);

        // Act
        var result = await this._sut.Register(request, It.IsAny<CancellationToken>());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(new Dictionary<string, List<string>> { { "errors", ["Email is already in use."] } },
            result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnAuthResult_WhenEmailDoesNotExist()
    {
        // Arrange
        var (_, request) = CreateUser();
        this._uowMock.Setup(u => u.Users.GetAll()).Returns(Enumerable.Empty<User>().AsQueryable());
        this._uowMock.Setup(u => u.Roles.GetAll()).Returns(this.CreateQueryableRoleMockEntities());
        this._passwordHasherMock.Setup(p =>
            p.CreatePasswordHash(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny));
        this._jwtTokenGeneratorMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        // Act
        var result = await this._sut.Register(request, CancellationToken.None);

        // Assert
        Assert.False(false);
        Assert.NotNull(result.Value);
        Assert.Equal("token", result.Value.Token);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenValidationFails()
    {
        // Arrange
        var (_, request) = CreateUser();
        this._mockValidator
            .Setup(v => v.ValidateAndExecuteAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterRequest, string>.Failure("Validation error"));

        // Act
        var result = await this._sut.Register(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(new Dictionary<string, List<string>> { { "errors", ["Validation error"] } }, result.Errors);
    }

    private static (IQueryable<User>, RegisterRequest) CreateUser()
    {
        var request =
            new RegisterRequest("John", "Doe", "JohnDoe@email.com",
                "password");
        var user = new List<User> { User.Create(request.FirstName, request.LastName, request.Email, null!) };

        return (user.AsQueryable(), request);
    }

    private IQueryable<Role> CreateQueryableRoleMockEntities()
    {
        return this.RepositoryUtils.MockEntitiesFromEnum<Roles, Role>();
    }
}