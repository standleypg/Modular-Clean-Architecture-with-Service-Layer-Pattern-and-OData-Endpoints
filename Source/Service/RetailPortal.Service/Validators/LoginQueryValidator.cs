using FluentValidation;
using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.Service.Validators;

public class LoginQueryValidator: AbstractValidator<LoginRequest>
{
      public LoginQueryValidator()
      {
          this.RuleFor(x => x.Email).NotEmpty();
          this.RuleFor(x => x.Password).NotEmpty();
      }
}