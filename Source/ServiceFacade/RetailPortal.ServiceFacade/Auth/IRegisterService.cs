using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.ServiceFacade.Auth;

public interface IRegisterService
{
    Task<AuthResult> Register(RegisterRequest command, CancellationToken cancellationToken = default);
}