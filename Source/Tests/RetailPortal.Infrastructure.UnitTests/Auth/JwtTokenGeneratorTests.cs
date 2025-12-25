using Microsoft.Extensions.Options;
using Moq;
using RetailPortal.Domain.Interfaces.Infrastructure.Services;
using RetailPortal.Infrastructure.Auth;
using RetailPortal.Infrastructure.UnitTests.Data.Repositories.Common;
using RetailPortal.Model.Constants;

namespace RetailPortal.Infrastructure.UnitTests.Auth;

public class JwtTokenGeneratorTests
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
        var user = RepositoryUtils.CreateUser(1);

        // Act
        var token = this._sut.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
    }
}