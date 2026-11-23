using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ArthaNiti.Infrastructure.Persistence;

namespace ArthaNiti.Infrastructure.Services;

public class DatabaseScriptService
{
    private readonly ArthaNitiDbContext _context;
    private readonly ILogger<DatabaseScriptService> _logger;

    public DatabaseScriptService(
        ArthaNitiDbContext context,
        ILogger<DatabaseScriptService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteScriptsAsync()
    {
        var folderPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Database Queries");
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException(
                $"Database Queries folder not found: {folderPath}");
        }

        var sqlFiles = Directory
            .GetFiles(folderPath, "*.sql")
            .OrderBy(file => file)
            .ToList();

        if (!sqlFiles.Any())
        {
            throw new FileNotFoundException(
                $"No SQL files found in: {folderPath}");
        }

        foreach (var file in sqlFiles)
        {
            var fileName = Path.GetFileName(file);

            try
            {
                _logger.LogInformation(
                    "Executing database script: {FileName}",
                    fileName);

                var sql = await File.ReadAllTextAsync(file);

                if (string.IsNullOrWhiteSpace(sql))
                {
                    _logger.LogWarning(
                        "Skipping empty database script: {FileName}",
                        fileName);

                    continue;
                }

                await _context.Database.ExecuteSqlRawAsync(sql);

                _logger.LogInformation(
                    "Database script executed successfully: {FileName}",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Database script execution failed: {FileName}",
                    fileName);

                throw;
            }
        }
    }
}
