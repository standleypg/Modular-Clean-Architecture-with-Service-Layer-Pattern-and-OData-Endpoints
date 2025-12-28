using FluentValidation;
using RetailPortal.DataFacade.Auth;
using RetailPortal.DataFacade.Data.UnitOfWork;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.ServiceFacade;
using RetailPortal.ServiceFacade.Auth;
using RetailPortal.ServiceFacade.Role;

namespace RetailPortal.Service.Services.Auth;

public class RegisterService(
    IUnitOfWork uow,
    IRoleService roleService,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IValidator<RegisterRequest> validator) : IRegisterService
{
    public async Task<Result<AuthResponse, string>> Register(RegisterRequest command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        if (uow.Users.GetAll().FirstOrDefault(u => u.Email == command.Email) is not null)
        {
            return Result<AuthResponse, string>.Failure("Email is already in use.");
        }

        passwordHasher.CreatePasswordHash(command.Password, out var passwordHash, out var passwordSalt);
        var password = Password.Create(passwordHash, passwordSalt);
        var user = User.Create(command.FirstName, command.LastName, command.Email, password);

        var role = await roleService.GetRoleByNameAsync(Roles.User);
        user.AddRole(role);

        uow.Users.Add(user);
        await uow.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(user);

        return AuthResponse.Create(user, token);
    }
}