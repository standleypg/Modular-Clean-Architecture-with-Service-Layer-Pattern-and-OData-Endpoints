using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RetailPortal.Model.Constants;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.ServiceFacade.Auth;

namespace RetailPortal.Api.Controllers;

[ApiVersion("0.0")]
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IRegisterService registerService, ILoginService loginService, ITokenExchangeService tokenExchangeService) : ODataController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await registerService.Register(request.Adapt<RegisterRequest>());

        return this.Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await loginService.Login(request.Adapt<LoginRequest>());

        if (result.IsSuccess)
        {
            return this.Ok(result.Value);
        }

        return this.Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Error);
    }

    [HttpGet("token-exchange")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = Appsettings.AzureAdSettings.JwtBearerScheme)]
    public async Task<IActionResult> TokenExchange()
    {
        var result = await tokenExchangeService.ExchangeToken(this.User.Adapt<TokenExchangeRequest>());

        return this.Ok(result.Adapt<AuthResponse>());
    }
}