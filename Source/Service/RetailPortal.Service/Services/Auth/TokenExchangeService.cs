using FluentValidation;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.ServiceFacade;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Role;

namespace RetailPortal.Service.Services.Auth;

public class TokenExchangeService(
    IUnitOfWork uow,
    IRoleService roleService,
    IJwtTokenGenerator jwtTokenGenerator,
    IValidator<TokenExchangeRequest> validator) : ITokenExchangeService
{
    public async Task<AuthResult> ExchangeToken(TokenExchangeRequest request, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var name = request.Name.AsSpan();
        var firstName = name[..name.IndexOf(' ')].ToString();
        var lastName = name[name.IndexOf(' ')..].ToString();
        var email = request.Email;

        if (uow.Users.GetAll().FirstOrDefault(u => u.Email == email) is not { } user)
        {
            var role = await roleService.GetRoleByNameAsync(Roles.User);
            var provider = request.TokenProvider == TokenProvider.Google.ToString()
                ? TokenProvider.Google
                : TokenProvider.Azure;
            user = User.Create(firstName, lastName, email, provider);
            user.AddRole(role);

            uow.Users.Add(user);
            await uow.SaveChangesAsync(cancellationToken);
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthResult(user, token);
    }
}