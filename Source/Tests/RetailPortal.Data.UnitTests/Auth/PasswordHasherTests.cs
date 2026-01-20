using RetailPortal.Data.Auth;

namespace RetailPortal.Infrastructure.UnitTests.Auth;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut = new();

    [Fact]
    public void CreatePasswordHash_ShouldCreatePasswordHash()
    {
        // Arrange
        var password = "password";

        // Act
        this._sut.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        // Assert
        Assert.NotNull(passwordHash);
        Assert.NotNull(passwordSalt);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var password = "password";
        this._sut.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        // Act
        var result = this._sut.VerifyPasswordHash(password, passwordHash, passwordSalt);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPasswordHash_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        // Arrange
        var password = "password";
        this._sut.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        // Act
        var result = this._sut.VerifyPasswordHash("incorrectPassword", passwordHash, passwordSalt);

        // Assert
        Assert.False(result);
    }
}