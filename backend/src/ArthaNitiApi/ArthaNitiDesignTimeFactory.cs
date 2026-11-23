using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArthaNiti.Api;

public sealed class ArthaNitiDesignTimeFactory : IDesignTimeDbContextFactory<ArthaNitiDbContext>
{
    public ArthaNitiDbContext CreateDbContext(string[] args)
    {
        LocalConfiguration.Load();
        var connection = Environment.GetEnvironmentVariable("DB_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection))
            throw new InvalidOperationException("Run Setup-Local.ps1 or configure DB_CONNECTION.");

        var options = new DbContextOptionsBuilder<ArthaNitiDbContext>()
            .UseSqlServer(connection)
            .Options;
        return new ArthaNitiDbContext(options);
    }
}
