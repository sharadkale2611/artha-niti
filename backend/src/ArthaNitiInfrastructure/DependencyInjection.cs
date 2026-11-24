using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence;
using ArthaNiti.Infrastructure.Services;

namespace ArthaNiti.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // --------------------------------------------
        // Database Connection
        // --------------------------------------------

        var connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DB_CONNECTION is not configured.");
        }
        services.AddScoped<ITenantUserService,TenantUserService>();


        services.AddDbContext<ArthaNitiDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // --------------------------------------------
        // ASP.NET Core Identity
        // --------------------------------------------

        services
            .AddIdentityCore<AppUser>(options => options.User.RequireUniqueEmail = true)
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<ArthaNitiDbContext>();

        // --------------------------------------------
        // Authentication Services
        // --------------------------------------------

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // --------------------------------------------
        // Database Services
        // --------------------------------------------

        services.AddScoped<DatabaseScriptService>();

        // --------------------------------------------
        // JWT Configuration
        // --------------------------------------------

        var jwtSecret =
            Environment.GetEnvironmentVariable("JWT_SECRET");

        var jwtIssuer =
            Environment.GetEnvironmentVariable("JWT_ISSUER");

        var jwtAudience =
            Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new InvalidOperationException(
                "JWT_SECRET is not configured.");
        }

        if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET must be at least 32 bytes long.");
        }

        if (string.IsNullOrWhiteSpace(jwtIssuer) ||
            string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new InvalidOperationException(
                "JWT_ISSUER and JWT_AUDIENCE must be configured.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claim = context.Principal?.FindFirst("tenant_id")?.Value;
                        if (!int.TryParse(claim, out var tenantId) || tenantId <= 0)
                            context.Fail("A positive tenant_id claim is required.");
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSecret)),

                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        return services;
    }
}
