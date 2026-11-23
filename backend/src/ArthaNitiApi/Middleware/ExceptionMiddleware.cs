using System.Net;
using System.Text.Json;

namespace ArthaNiti.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (!context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogError(ex, "An unhandled exception occurred. Path: {Path}", context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            var isBadRequest = ex is InvalidOperationException;
            context.Response.StatusCode = (int)(isBadRequest
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError);
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new
            {
                success = false,
                message = isBadRequest ? ex.Message : "An unexpected error occurred.",
                data = (object?)null,
                errors = Array.Empty<string>()
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response), context.RequestAborted);
        }
    }
}
