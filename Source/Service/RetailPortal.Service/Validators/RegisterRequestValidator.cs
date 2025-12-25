using FluentValidation;
using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.Service.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        this.RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        this.RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        this.RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}