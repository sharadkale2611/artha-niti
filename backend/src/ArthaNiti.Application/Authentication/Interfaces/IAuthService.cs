using ArthaNiti.Application.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(
     RegisterRequest request,
     int tenantId);

        Task<AuthResponse> LoginAsync(LoginRequest request);

        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task LogoutAsync(LogoutRequest request);
    }
}
