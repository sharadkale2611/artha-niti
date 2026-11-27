
using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Application.Menus.Interfaces;
using ArthaNiti.Application.TenantAuditLogs.Interfaces;
using ArthaNiti.Application.TenantRolePermissions.Interfaces;
using ArthaNiti.Application.TenantSettings.Interfaces;
using ArthaNiti.Application.TenantUserRoles.Interfaces;
using ArthaNiti.Application.TenantUsers.Interfaces;
using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence;
using ArthaNiti.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace ArthaNiti.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==========================================
        // DATABASE CONNECTION
        // ==========================================

        var connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DB_CONNECTION is not configured.");
        }

        services.AddDbContext<ArthaNitiDbContext>(options =>
            options.UseSqlServer(connectionString));


        // ==========================================
        // DATABASE SERVICES
        // ==========================================

        services.AddScoped<DatabaseScriptService>();


        // ==========================================
        // IDENTITY CONFIGURATION
        // ==========================================

        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<ArthaNitiDbContext>()
            .AddDefaultTokenProviders();


        // ==========================================
        // JWT SETTINGS
        // ==========================================

        var jwtSettings = new JwtSettings
        {
            Key = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException(
                    "JWT_SECRET is not configured."),

            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? "ArthaNiti",

            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? "ArthaNitiClient",

            ExpiryMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES"),
                out var expiry)
                ? expiry
                : 60
        };

        if (Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
        {
            throw new InvalidOperationException(
                "JWT_SECRET must be at least 32 bytes long.");
        }

        services.Configure<JwtSettings>(options =>
        {
            options.Key = jwtSettings.Key;
            options.Issuer = jwtSettings.Issuer;
            options.Audience = jwtSettings.Audience;
            options.ExpiryMinutes = jwtSettings.ExpiryMinutes;
        });


        // ==========================================
        // JWT TOKEN SERVICE
        // ==========================================

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        
       

        // ==========================================
        // AUTH SERVICE
        // ==========================================

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantUserService, TenantUserService>();
        services.AddScoped<ITenantUserRoleService, TenantUserRoleService>();
        services.AddScoped<ITenantSettingService, TenantSettingService>();
<<<<<<< HEAD
        services.AddScoped<ITenantRolePermissionService, TenantRolePermissionService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ITenantAuditLogService, TenantAuditLogService>();
=======
        services.AddScoped<IPermissionService, PermissionService>();
>>>>>>> 1129377fa0e6c466ca25372a70ee69c263dd1055


        // ==========================================
        // JWT AUTHENTICATION
        // ==========================================

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                    ClockSkew = TimeSpan.Zero
                };
        });


        // ==========================================
        // AUTHORIZATION
        // ==========================================

        services.AddAuthorization();

        return services;
    }
}
