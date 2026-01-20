using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.ServiceFacade.Auth;

public interface IRegisterService
{
    Task<Result<AuthResponse, string>> Register(RegisterRequest request, CancellationToken cancellationToken = default);
}