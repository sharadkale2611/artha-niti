using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;

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

    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        try
        {
            if (!int.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId) || tenantId <= 0)
                return Forbid();

            var response = await _authService.RegisterAsync(request, tenantId);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request)
    {
        try
        {
            await _authService.LogoutAsync(request);

            return Ok(new
            {
                message = "Logout successful."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        userId = User.FindFirst("sub")?.Value,
        tenantId = User.FindFirst("tenant_id")?.Value,
        roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
    });

    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("firm-admin")]
    public IActionResult FirmAdmin() => Ok(new { message = "Firm admin access granted." });
}
