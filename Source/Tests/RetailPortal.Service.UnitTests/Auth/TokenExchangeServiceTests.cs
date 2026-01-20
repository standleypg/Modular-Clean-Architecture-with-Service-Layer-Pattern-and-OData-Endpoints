using Moq;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Service.Services.Auth;
using RetailPortal.ServiceFacade.Validator.Common;

namespace RetailPortal.Service.UnitTests.Auth;

public class TokenExchangeServiceTests
{
    private readonly TokenExchangeService _sut;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IValidator> _validatorMock;

    public TokenExchangeServiceTests()
    {
        this._uowMock = new Mock<IUnitOfWork>();
        this._jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        this._validatorMock = new Mock<IValidator>();

        this._sut = new TokenExchangeService(this._uowMock.Object, this._jwtTokenGeneratorMock.Object,
            this._validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnAuthResult_WhenUserDoesNotExist()
    {
        // Arrange
        var (user, request) = CreateUser();
        this._uowMock.Setup(u => u.Users.GetAll()).Returns(user);
        this._jwtTokenGeneratorMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        // Act
        var result = await this._sut.ExchangeToken(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("token", result.Value.Token);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResult_WhenUserExist()
    {
        // Arrange
        var (user, request) = CreateUser();
        this._uowMock.Setup(u => u.Users.GetAll()).Returns(user);
        this._jwtTokenGeneratorMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        // Act
        var result = await this._sut.ExchangeToken(request, It.IsAny<CancellationToken>());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("token", result.Value.Token);
    }

    private static (IQueryable<User>, TokenExchangeRequest) CreateUser()
    {
        var request = new TokenExchangeRequest("JohnDoe@email.com", "John Doe", "Google");
        var name = request.Name.AsSpan();
        var users = new List<User>
        {
            User.Create(name[..name.IndexOf(' ')].ToString(), name[name.IndexOf(' ')..].ToString(),
                request.Email,
                null!)
        };
        return (users.AsQueryable(), request);
    }
}