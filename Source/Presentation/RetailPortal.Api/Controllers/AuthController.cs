using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using RetailPortal.Model.Constants;
using RetailPortal.Model.DTOs.Auth;
using RetailPortal.Model.DTOs.Common;
using RetailPortal.ServiceFacade.Auth;
using System.Security.Claims;

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

        return result
            .Match(
                onSuccess: this.Ok,
                onFailure: error => this.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: error)
            );
    }

    [HttpGet("token-exchange")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = Appsettings.AzureAdSettings.JwtBearerScheme)]
    public async Task<IActionResult> TokenExchange()
    {
        var tokenExchangeRequest = new TokenExchangeRequest
        (
            this.User.GetClaimValue(CustomClaimTypes.Email, ClaimTypes.Email),
            this.User.GetClaimValue(CustomClaimTypes.Name),
            this.User.GetClaimValue(CustomClaimTypes.Iss)
        );
        var result = await tokenExchangeService.ExchangeToken(tokenExchangeRequest);
        var t = this.Ok();

        return result
            .Match(
                onSuccess: this.Ok,
                onFailure: error => this.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: error)
            );
    }
}