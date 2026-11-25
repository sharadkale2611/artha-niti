using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var tenantId = int.TryParse(
            Environment.GetEnvironmentVariable("DEFAULT_TENANT_ID"),
            out var configuredTenantId)
                ? configuredTenantId
                : 1;

        var response = await _authService.RegisterAsync(
            request, tenantId);

        return Ok(new
        {
            Success = true,
            Message = "Registration successful.",
            Data = response
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        return Ok(new
        {
            Success = true,
            Message = "Login successful.",
            Data = response
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        return Ok(new
        {
            Success = true,
            Message = "Token refreshed successfully.",
            Data = response
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        await _authService.LogoutAsync(request);

        return Ok(new
        {
            Success = true,
            Message = "Logout successful."
        });
    }
}