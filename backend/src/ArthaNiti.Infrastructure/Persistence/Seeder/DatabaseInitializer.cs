
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

        // --------------------------------------------
        // Apply EF Core migrations only when model
        // has no pending model changes
        // --------------------------------------------

        //if (!dbContext.Database.HasPendingModelChanges())
        //{
        //    await dbContext.Database.MigrateAsync();
        //}


        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();

        if (!appliedMigrations.Any())
        {
            var initialMigration = dbContext.Database.GetMigrations().First();
            await dbContext.Database.MigrateAsync(initialMigration);
        }
        // --------------------------------------------
        // Execute database SQL scripts
        // --------------------------------------------

        var databaseScriptService =
            scope.ServiceProvider
                .GetRequiredService<DatabaseScriptService>();

        await databaseScriptService.ExecuteScriptsAsync();

        // --------------------------------------------
        // Seed roles and initial Super Admin
        // --------------------------------------------

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
