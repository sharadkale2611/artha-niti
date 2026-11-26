using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers;

//  [Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public class TenantController : ControllerBase
{
    private readonly ITenantService _service;
    private readonly ILogger<TenantController> _logger;
    private readonly IWebHostEnvironment _environment;

    public TenantController(
        ITenantService service,
        ILogger<TenantController> logger,
        IWebHostEnvironment environment)
    {
        _service = service;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var tenants = await _service.GetAllAsync(cancellationToken);
            return Ok(Success("Records retrieved successfully.", tenants));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return ServerError(ex);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(Failure("Tenant ID must be greater than zero."));

        try
        {
            var tenant = await _service.GetByIdAsync(id, cancellationToken);

            if (tenant is null)
                return NotFound(Failure($"Tenant {id} was not found."));

            return Ok(Success("Record retrieved successfully.", tenant));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return ServerError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var tenant = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = tenant.TenantId },
                Success("Record created successfully.", tenant));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return ServerError(ex);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TenantUpdateDto dto,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(Failure("Tenant ID must be greater than zero."));

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return NotFound(Failure($"Tenant {id} was not found."));

            var tenant = await _service.GetByIdAsync(id, cancellationToken);

            if (tenant is null)
                return NotFound(Failure($"Tenant {id} was not found."));

            return Ok(Success("Record updated successfully.", tenant));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return ServerError(ex);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest(Failure("Tenant ID must be greater than zero."));

        try
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound(Failure($"Tenant {id} was not found."));

            return Ok(Success("Record deleted successfully.", true));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return ServerError(ex);
        }
    }

    private IActionResult ServerError(Exception exception)
    {
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Tenant API failed for {Method} {Path}. Trace ID {TraceId}",
            Request.Method,
            Request.Path,
            traceId);

        if (_environment.IsDevelopment())
        {
            var cause = exception.GetBaseException();

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<object>
                {
                    Success = false,
                    Message = cause.Message,
                    Data = null,
                    Errors = new List<string>
                    {
                        $"{cause.GetType().Name}: {cause.Message}"
                    }
                });
        }

        return StatusCode(
            StatusCodes.Status500InternalServerError,
            Failure($"Request failed. Trace ID {traceId}."));
    }

    private static ApiResponse<T> Success<T>(string message, T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    private static ApiResponse<object> Failure(string message)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = new List<string> { message }
        };
    }
}