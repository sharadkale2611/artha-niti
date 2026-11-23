using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence.Seeder;
using ArthaNiti.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArthaNiti.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ArthaNitiDbContext>();

        // The schema is managed explicitly in Package Manager Console.
        if (!dbContext.Database.GetMigrations().Any() ||
            (await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            throw new InvalidOperationException("Create the initial migration and run Update-Database before starting the API.");
        }

        // Execute database SQL scripts
        var databaseScriptService =
            scope.ServiceProvider
                .GetRequiredService<DatabaseScriptService>();

        await databaseScriptService.ExecuteScriptsAsync();

        // Seed roles and initial Super Admin
        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<AppRole>>();

        await DatabaseSeeder.SeedAsync(
            dbContext,
            userManager,
            roleManager);
    }
}
