using Asp.Versioning;
using AutoMapper;
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
public class AuthController(IMapper mapper, IRegisterService registerService, ILoginService loginService, ITokenExchangeService tokenExchangeService) : ODataController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await registerService.Register(mapper.Map<RegisterRequest>(request));

        return this.Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await loginService.Login(mapper.Map<LoginRequest>(request));

        return this.Ok(mapper.Map<AuthResponse>(result));
    }

    [HttpGet("token-exchange")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = Appsettings.AzureAdSettings.JwtBearerScheme)]
    public async Task<IActionResult> TokenExchange()
    {
        var result = await tokenExchangeService.ExchangeToken(mapper.Map<TokenExchangeRequest>(this.User));

        return this.Ok(mapper.Map<AuthResponse>(result));
    }
}