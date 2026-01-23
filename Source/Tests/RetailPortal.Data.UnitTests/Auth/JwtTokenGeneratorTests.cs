using Microsoft.Extensions.Options;
using Moq;
using RetailPortal.Data.Auth;
using RetailPortal.DataFacade.Services;
using RetailPortal.Infrastructure.UnitTests.Common;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Infrastructure.UnitTests.Auth;

public class JwtTokenGeneratorTests: ServiceTestBase
{
    private readonly JwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        Mock<IDateTimeProvider> dateTimeProviderMock = new();
        Mock<IOptions<Appsettings.JwtSettings>> jwtOptionsMock = new();
        dateTimeProviderMock.Setup(d => d.UtcNow).Returns(DateTime.Now);
        jwtOptionsMock.Setup(j => j.Value).Returns(new Appsettings.JwtSettings
        {
            Secret = "long secret for HMACSHA512 algorithm to be used for signing the JWT token",
            ExpirationMinutes = 30
        });
        this._sut = new JwtTokenGenerator(dateTimeProviderMock.Object, jwtOptionsMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnToken()
    {
        // Arrange
        var user = this.RepositoryUtils.MockEntities<User>();

        // Act
        var token = this._sut.GenerateToken(user.First());

        // Assert
        Assert.NotNull(token);
    }
}