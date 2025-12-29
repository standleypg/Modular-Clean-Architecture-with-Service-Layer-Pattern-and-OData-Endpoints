using MapsterMapper;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.Repositories;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Validator.Common;

namespace RetailPortal.Service.Services.Auth;

public class LoginService(
    IReadStore readStore,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IValidator validator) : ILoginService
{
    public async Task<Result<AuthResponse, string>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        if (await validator.ValidateAndExecuteAsync(request, cancellationToken) is
            { IsSuccess: false } validationResult)
        {
            return validationResult.ConvertFailure<LoginRequest, AuthResponse>();
        }

        if (readStore.User.GetAll().FirstOrDefault(u => u.Email == request.Email) is not { } user)
        {
            return Result<AuthResponse, string>.Failure("Invalid credentials.");
        }

        if (!passwordHasher.VerifyPasswordHash(request.Password, user.Password!.PasswordHash,
                user.Password!.PasswordSalt))
        {
            return Result<AuthResponse, string>.Failure("Invalid credentials.");
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        return AuthResponse.Create(user, token);
    }
}