using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.ServiceFacade.Auth;

namespace RetailPortal.Service.Services.Auth;

public class TokenExchangeService(
    IUnitOfWork uow,
    IJwtTokenGenerator jwtTokenGenerator,
    IValidator<TokenExchangeRequest> validator) : ITokenExchangeService
{
    public async Task<Result<AuthResponse, string>> ExchangeToken(TokenExchangeRequest request, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var name = request.Name.AsSpan();
        var firstName = name[..name.IndexOf(' ')].ToString();
        var lastName = name[(name.IndexOf(' ') + 1)..].ToString();
        var email = request.Email;

        if (uow.Users.GetAll().FirstOrDefault(u => u.Email == email) is not { } user)
        {
            var role = await uow.Roles.GetAll().FirstAsync(x => x.Name == nameof(Roles.User), cancellationToken: cancellationToken);
            var provider = request.TokenProvider.Contains(nameof(TokenProvider.Google), StringComparison.OrdinalIgnoreCase)
                ? TokenProvider.Google
                : TokenProvider.Azure;
            user = User.Create(firstName, lastName, email, provider);

            user.AddRole(role);

            uow.Users.Add(user);
            await uow.SaveChangesAsync(cancellationToken);
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        return AuthResponse.Create(user, token);
    }
}