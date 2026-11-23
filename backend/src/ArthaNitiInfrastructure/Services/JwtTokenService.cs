using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ArthaNiti.Application.Authentication.Interfaces;

namespace ArthaNiti.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpiryMinutes;

    public JwtTokenService()
    {
        _jwtSecret =
            Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new InvalidOperationException(
                "JWT_SECRET is not configured.");

        _jwtIssuer =
            Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? "ArthaNiti";

        _jwtAudience =
            Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? "ArthaNitiClient";

        _jwtExpiryMinutes =
            int.TryParse(
                Environment.GetEnvironmentVariable(
                    "JWT_EXPIRY_MINUTES"),
                out var expiry) && expiry > 0
                ? expiry
                : 60;

        if (Encoding.UTF8.GetByteCount(_jwtSecret) < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET must be at least 32 bytes long.");
        }
    }

    public Task<string> GenerateTokenAsync(
        string userId,
        string email,
        string fullName,
        int tenantId,
        IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim("tenant_id", tenantId.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email),

            new Claim(
                ClaimTypes.Name,
                fullName),

            new Claim(
                ClaimTypes.NameIdentifier,
                userId)
        };

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSecret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtExpiryMinutes),
            signingCredentials: credentials);

        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return Task.FromResult(tokenString);
    }
}
