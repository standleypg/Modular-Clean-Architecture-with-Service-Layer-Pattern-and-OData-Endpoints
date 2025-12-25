using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.ServiceFacade.Auth;

public interface ILoginService
{
    Task<AuthResult> Login(LoginRequest request, CancellationToken cancellationToken = default);
}