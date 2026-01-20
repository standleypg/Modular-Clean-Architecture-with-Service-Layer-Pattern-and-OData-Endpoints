using Microsoft.EntityFrameworkCore;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Validator.Common;

namespace RetailPortal.Service.Services.Auth;

public class RegisterService(
    IUnitOfWork uow,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IValidator validator) : IRegisterService
{
    public async Task<Result<AuthResponse, string>> Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if(await validator.ValidateAndExecuteAsync(request, cancellationToken) is { IsSuccess: false} validationError)
        {
            return validationError.ConvertFailure<RegisterRequest, AuthResponse>();
        }

        if (uow.Users.GetAll().FirstOrDefault(u => u.Email == request.Email) is not null)
        {
            return Result<AuthResponse, string>.Failure("Email is already in use.");
        }

        passwordHasher.CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);
        var password = Password.Create(passwordHash, passwordSalt);
        var user = User.Create(request.FirstName, request.LastName, request.Email, password);

        var role = await uow.Roles.GetAll().FirstAsync(x => x.Name == nameof(Roles.User), cancellationToken);
        user.AddRole(role);

        uow.Users.Add(user);
        await uow.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(user);

        return AuthResponse.Create(user, token);
    }
}