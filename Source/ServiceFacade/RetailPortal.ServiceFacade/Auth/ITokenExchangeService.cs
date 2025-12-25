using RetailPortal.Model.DTOs.Auth;

namespace RetailPortal.ServiceFacade.Auth;

public interface ITokenExchangeService
{
    Task<AuthResult> ExchangeToken(TokenExchangeRequest request, CancellationToken cancellationToken = default);
}