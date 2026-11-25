
using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArthaNiti.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public Task<string> GenerateTokenAsync(
        string userId,
        string email,
        string fullName,
        int tenantId,
        IList<string> roles)
    {
        // --------------------------------------------
        // Claims
        // --------------------------------------------

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),

            new Claim(JwtRegisteredClaimNames.Email, email),

            new Claim(ClaimTypes.Name, fullName),

            new Claim(ClaimTypes.NameIdentifier, userId),

            new Claim("TenantId", tenantId.ToString())
        };

        // --------------------------------------------
        // Roles
        // --------------------------------------------

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // --------------------------------------------
        // Signing Key
        // --------------------------------------------

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // --------------------------------------------
        // Create JWT Token
        // --------------------------------------------

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        // --------------------------------------------
        // Convert Token to String
        // --------------------------------------------

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Task.FromResult(tokenString);
    }
}
