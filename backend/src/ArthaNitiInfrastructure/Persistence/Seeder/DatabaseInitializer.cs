using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence.Seeder;
using ArthaNiti.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArthaNiti.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ArthaNitiDbContext>();

        if (!dbContext.Database.GetMigrations().Any())
        {
            throw new InvalidOperationException(
                "No EF Core migrations were found in ArthaNiti.Infrastructure. Pull the migration files or create the initial migration.");
        }

        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (environment.IsDevelopment())
        {
            await dbContext.Database.MigrateAsync();
        }
        else if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            throw new InvalidOperationException(
                "Apply the pending ArthaNiti migrations before starting the API in this environment.");
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



