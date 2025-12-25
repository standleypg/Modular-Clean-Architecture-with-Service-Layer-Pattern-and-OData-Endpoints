using AutoMapper;
using RetailPortal.Model.Constants;
using RetailPortal.Model.DTOs.Auth;
using System.Security.Claims;

namespace RetailPortal.Api.Common.Mapping;

public class AuthMappingConfig : Profile
{
    public AuthMappingConfig()
    {
        this.CreateMap<RegisterRequest, RegisterRequest>();

        this.CreateMap<LoginRequest, LoginRequest>();

        this.CreateMap<AuthResult, AuthResponse>()
            .ConstructUsing(auth => new AuthResponse(auth.User.Id, auth.User.FirstName, auth.User.LastName, auth.User.Email, auth.Token));

        this.CreateMap<ClaimsPrincipal, TokenExchangeRequest>()
            .ConstructUsing(user => new TokenExchangeRequest(
                user.FindFirst(CustomClaimTypes.Email) != null ? user.FindFirst(CustomClaimTypes.Email)!.Value : user.FindFirst(ClaimTypes.Email)!.Value,
                user.FindFirst(CustomClaimTypes.Name)!.Value,
                user.FindFirst(CustomClaimTypes.Iss)!.Value
            ));
    }
}