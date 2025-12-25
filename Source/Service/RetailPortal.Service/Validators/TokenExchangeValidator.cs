using FluentValidation;
using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.Service.Validators;

public class TokenExchangeValidator: AbstractValidator<TokenExchangeRequest>
{
    public TokenExchangeValidator()
    {
        this.RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        this.RuleFor(x => x.Name)
            .NotEmpty();

        this.RuleFor(x => x.TokenProvider)
            .NotEmpty();
    }
}