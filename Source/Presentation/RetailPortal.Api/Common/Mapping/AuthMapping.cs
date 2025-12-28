using Mapster;
using RetailPortal.Model.Constants;
using RetailPortal.Model.Db.Entities;
using RetailPortal.Model.DTOs.Auth;
using System.Security.Claims;

namespace RetailPortal.Api.Common.Mapping;

public class AuthMappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterRequest>();

        config.NewConfig<LoginRequest, LoginRequest>();

        config.NewConfig<User, AuthResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Token, src => MapContext.Current == null ? string.Empty : MapContext.Current.Parameters["Token"]);

        config.NewConfig<ClaimsPrincipal, TokenExchangeRequest>()
            .Map(dest => dest.Email, src => src.FindFirst(CustomClaimTypes.Email) != null ? src.FindFirst(CustomClaimTypes.Email)!.Value : src.FindFirst(ClaimTypes.Email)!.Value)
            .Map(dest => dest.Name, src => src.FindFirst(CustomClaimTypes.Name)!.Value)
            .Map(dest => dest.TokenProvider, src => src.FindFirst(CustomClaimTypes.Iss)!.Value);
    }
}