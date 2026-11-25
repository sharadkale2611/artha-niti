using ArthaNiti.Application.Authentication;
using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
//using Microsoft.AspNetCore.Identity;
using System.Text;

namespace ArthaNiti.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ArthaNitiDbContext _context;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IJwtTokenService jwtTokenService,
        ArthaNitiDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _context = context;
    }

    // REGISTER

    // REGISTER
    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        int tenantId)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Invalid TenantId.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered.");

        var defaultRole = Environment.GetEnvironmentVariable("DEFAULT_USER_ROLE")
            ?? "User";

        var role = await _roleManager.FindByNameAsync(defaultRole);

        if (role == null)
            throw new InvalidOperationException(
                $"Role '{defaultRole}' does not exist.");

        if (role.TenantId != tenantId)
            throw new InvalidOperationException(
                "The selected role does not belong to this tenant.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var user = new AppUser
        {
            FullName = request.FullName,
            UserName = request.Email,
            Email = request.Email,
            TenantId = tenantId,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, defaultRole);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errors);
        }

        var roles = await _userManager.GetRolesAsync(user);

        await transaction.CommitAsync();

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            TenantId = user.TenantId,
            Roles = roles
        };
    }


    // LOGIN
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordValid = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, false);

        if (!passwordValid.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");

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

        await _userManager.UpdateAsync(user);

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

    // REFRESH TOKEN
    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenRequest request)
    {
        var hashedToken = HashRefreshToken(request.RefreshToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);

        if (user == null ||
            user.RefreshTokenExpiryTime == null ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidRefreshTokenException();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = await _jwtTokenService.GenerateTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            user.TenantId,
            roles);

        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

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

    // LOGOUT
    public async Task LogoutAsync(LogoutRequest request)
    {
        var hashedToken = HashRefreshToken(request.RefreshToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedToken);

        if (user == null)
            throw new InvalidRefreshTokenException();

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);
    }

    // GENERATE REFRESH TOKEN
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    // HASH REFRESH TOKEN
    private static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(bytes);
    }
}