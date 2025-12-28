using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.ServiceFacade.Auth;

public interface ILoginService
{
    Task<Result<AuthResponse, string>> Login(LoginRequest request, CancellationToken cancellationToken = default);
}