using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ArthaNiti.Infrastructure.Identity;

namespace ArthaNiti.Infrastructure.Persistence.Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ArthaNitiDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        var configuredRoles = Environment.GetEnvironmentVariable("IDENTITY_ROLES") ?? string.Empty;
        var roles = new[] { "SuperAdmin", "Admin", "User" }
            .Union(configuredRoles.Split(',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                StringComparer.OrdinalIgnoreCase);

        using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new AppRole
                    {
                        Name = role,
                        TenantId = 1
                    });

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {errors}");
                }
            }
        }

        // --------------------------------------------
        // Initial Super Admin
        // --------------------------------------------

        var adminEmail =
            Environment.GetEnvironmentVariable("ADMIN_EMAIL")
            ?? "admin@arthaniti.test";

        var existingAdmin =
            await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            // --------------------------------------------
            // Create initial Super Admin user
            // --------------------------------------------

            var adminPassword =
                Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "ADMIN_PASSWORD is not configured.");
            }

            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "ArthaNiti Super Admin",
                TenantId = 1,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create initial Super Admin user: {errors}");
            }

            // --------------------------------------------
            // Assign Super Admin role
            // --------------------------------------------

            var roleResult = await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign Super Admin role: {errors}");
            }
        }
        await transaction.CommitAsync();
    }
}



