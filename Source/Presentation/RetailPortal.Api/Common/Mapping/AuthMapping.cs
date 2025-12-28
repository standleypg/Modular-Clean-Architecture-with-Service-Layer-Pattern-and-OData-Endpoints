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
    }
}