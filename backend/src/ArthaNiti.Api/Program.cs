using ArthaNiti.Api.Middleware;
using ArthaNiti.Infrastructure;
using ArthaNiti.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

builder.Services.AddControllers();

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

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ArthaNiti API",
        Version = "v1",
        Description = "ArthaNiti Backend API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AngularClient");

await DatabaseInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "ArthaNiti API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        success = true,
        message = "ArthaNiti API is running successfully.",
        application = "ArthaNiti API",
        version = "v1",
        environment = app.Environment.EnvironmentName,
        timestamp = DateTime.UtcNow
    });
});

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        application = "ArthaNiti API",
        timestamp = DateTime.UtcNow
    });
});

app.Run();
