using ArthaNiti.Api;
using ArthaNiti.Api.Middleware;
using ArthaNiti.Infrastructure;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.OpenApi;
using Serilog;

LocalConfiguration.Load();

var logLevel =
    Environment.GetEnvironmentVariable("LOG_LEVEL")
    ?? "Information";

var efCoreLogLevel =
    Environment.GetEnvironmentVariable("EF_CORE_LOG_LEVEL")
    ?? "Warning";

if (!Enum.TryParse<Serilog.Events.LogEventLevel>(
        logLevel, true, out var applicationLogLevel))
{
    applicationLogLevel =
        Serilog.Events.LogEventLevel.Information;
}

if (!Enum.TryParse<Serilog.Events.LogEventLevel>(
        efCoreLogLevel, true, out var efCoreLevel))
{
    efCoreLevel =
        Serilog.Events.LogEventLevel.Warning;
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(applicationLogLevel)
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore.Database.Command",
        efCoreLevel)
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Swagger - Swashbuckle 10.x
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ArthaNiti API",
            Version = "v1",
            Description = "ArthaNiti Backend API"
        });

        options.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token."
            });

        options.AddSecurityRequirement(document =>
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(
                    "Bearer", document)] = []
            });
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AngularClient", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Database, Identity and JWT
    builder.Services.AddInfrastructure();

    var app = builder.Build();
    app.UseMiddleware<ExceptionMiddleware>();

    // Database Initialization
    await DatabaseInitializer.InitializeAsync(app.Services);


    // Swagger UI
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors("AngularClient");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "ArthaNiti API terminated unexpectedly.");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
