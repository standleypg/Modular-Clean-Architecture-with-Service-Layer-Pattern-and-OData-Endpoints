using FluentValidation.TestHelper;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Service.Validators;

namespace RetailPortal.Service.UnitTests.Auth;

public class TokenExchangeValidatorTests
{
    private readonly TokenExchangeValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new TokenExchangeRequest(string.Empty, "Name", "Provider");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new TokenExchangeRequest("invalid email", "Name", "Provider");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var model = new TokenExchangeRequest("test@example.com", "Name", "Provider");
        var result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new TokenExchangeRequest("test@example.com", string.Empty, "Provider");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Not_Empty()
    {
        var model = new TokenExchangeRequest("test@example.com", "Name", "Provider");
        var result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_TokenProvider_Is_Empty()
    {
        var model = new TokenExchangeRequest("test@example.com", "Name", string.Empty);
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TokenProvider);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TokenProvider_Is_Not_Empty()
    {
        var model = new TokenExchangeRequest("test@example.com", "Name", "TokenProvider");
        var result = this._validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TokenProvider);
    }
}