using FluentValidation;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.ServiceFacade.Auth;

namespace RetailPortal.Service.Services.Auth;

public class LoginService(IReadStore readStore, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IValidator<LoginRequest> validator): ILoginService
{
    public async Task<AuthResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (readStore.User.GetAll().FirstOrDefault(u => u.Email == request.Email) is not { } user)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        if (!passwordHasher.VerifyPasswordHash(request.Password, user.Password!.PasswordHash, user.Password!.PasswordSalt))
        {
            throw new InvalidOperationException("Invalid credentials."); // just an example of returning list of errors
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthResult(user, token);
    }
}