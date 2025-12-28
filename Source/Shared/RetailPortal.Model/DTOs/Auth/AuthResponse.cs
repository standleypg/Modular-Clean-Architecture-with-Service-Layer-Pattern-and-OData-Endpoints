using Mapster;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.Model.DTOs.Auth;

public record AuthResponse(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string Token
);

public static class AuthResponseExtensions
{
    extension(AuthResponse)
    {
        public static Result<AuthResponse, string> Create(User user, string token)
        {
            var authResponse = user.Adapt<AuthResponse>() with { Token = token };
            return Result<AuthResponse, string>.Success(authResponse);
        }
    }
}