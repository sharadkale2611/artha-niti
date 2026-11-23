using ArthaNiti.Application.Authentication.DTOs;

namespace ArthaNiti.Application.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, int tenantId);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

    Task LogoutAsync(LogoutRequest request);
}
