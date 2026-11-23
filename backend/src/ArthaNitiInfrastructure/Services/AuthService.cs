using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence;

namespace ArthaNiti.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ArthaNitiDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        ArthaNitiDbContext db,
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request, int tenantId)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "User with this email already exists.");
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            TenantId = tenantId
        };

        await using var registration = await _db.Database.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        const string defaultRole = "User";

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            defaultRole);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to assign default role: {errors}");
        }

        var roles = await _userManager.GetRolesAsync(user);

        await registration.CommitAsync();
        return new AuthResponse
        {
            Token = null,
            RefreshToken = null,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            TenantId = user.TenantId,
            Roles = roles
        };
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(
            request.Email);

        if (user == null ||
            !await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            throw new InvalidOperationException(
                "Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = await _jwtTokenService.GenerateTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            user.TenantId,
            roles);

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = HashRefreshToken(refreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                updateResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to update refresh token: {errors}");
        }

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            TenantId = user.TenantId,
            Roles = roles
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenRequest request)
    {
        var tokenHash = HashRefreshToken(request.RefreshToken);
        var user = await _userManager.Users
            .FirstOrDefaultAsync(
                u => u.RefreshToken == tokenHash);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Invalid refresh token.");
        }

        if (user.RefreshTokenExpiryTime == null ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Refresh token has expired.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken =
            await _jwtTokenService.GenerateTokenAsync(
                user.Id,
                user.Email!,
                user.FullName,
                user.TenantId,
                roles);

        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                updateResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to rotate refresh token: {errors}");
        }

        return new AuthResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            TenantId = user.TenantId,
            Roles = roles
        };
    }

    private static string HashRefreshToken(string value)
    {
        return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public async Task LogoutAsync(LogoutRequest request)
    {
        var tokenHash = HashRefreshToken(request.RefreshToken);
        var user = await _userManager.Users
            .FirstOrDefaultAsync(
                u => u.RefreshToken == tokenHash);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Invalid refresh token.");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                updateResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to logout user: {errors}");
        }
    }
}
