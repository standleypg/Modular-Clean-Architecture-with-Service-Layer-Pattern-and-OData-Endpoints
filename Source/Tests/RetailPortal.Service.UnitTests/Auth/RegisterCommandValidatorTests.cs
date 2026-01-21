using FluentValidation.TestHelper;
using RetailPortal.Service.Validators;

namespace RetailPortal.Service.UnitTests.Auth;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var model = new Model.DTOs.Auth.RegisterRequest(string.Empty, "Doe", "email@email.com", "password");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var model = new Model.DTOs.Auth.RegisterRequest("John", string.Empty, "email@email.com", "password");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new Model.DTOs.Auth.RegisterRequest("John", "Doe", "not_a_valid_email", "password");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new Model.DTOs.Auth.RegisterRequest("John", "Doe", "email@email.com", "pass");
        var result = this._validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new Model.DTOs.Auth.RegisterRequest("John", "Doe", "john.doe@example.com", "password123");
        var result = this._validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}