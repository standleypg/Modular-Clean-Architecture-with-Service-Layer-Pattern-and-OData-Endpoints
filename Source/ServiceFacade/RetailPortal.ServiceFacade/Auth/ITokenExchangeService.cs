using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;

namespace RetailPortal.ServiceFacade.Auth;

public interface ITokenExchangeService
{
    Task<Result<AuthResponse, string>> ExchangeToken(TokenExchangeRequest request, CancellationToken cancellationToken = default);
}