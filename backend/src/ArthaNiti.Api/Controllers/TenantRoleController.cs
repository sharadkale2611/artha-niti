using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ArthaNiti.Api.Controllers;

[Authorize(Roles = "FirmAdmin,SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
public class TenantRoleController : ControllerBase
{
    private readonly ITenantRoleService _service;
    private readonly ILogger<TenantRoleController> _logger;
    private readonly IWebHostEnvironment _environment;

    public TenantRoleController(
        ITenantRoleService service,
        ILogger<TenantRoleController> logger,
        IWebHostEnvironment environment)
    {
        _service = service;
        _logger = logger;
        _environment = environment;
    }

    // GET /api/TenantRole
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _service.GetAllAsync(cancellationToken);

            return Ok(Success("Tenant roles retrieved successfully.", roles));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    // GET /api/TenantRole/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return InvalidRoleId();

        try
        {
            var role = await _service.GetByIdAsync(id, cancellationToken);

            if (role is null)
                return RoleNotFound(id);

            return Ok(Success("Tenant role retrieved successfully.", role));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    // POST /api/TenantRole
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantRoleCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = role.TenantRoleId },
                Success("Tenant role created successfully.", role));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    // PUT /api/TenantRole/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TenantRoleUpdateDto dto,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return InvalidRoleId();

        try
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return RoleNotFound(id);

            var role = await _service.GetByIdAsync(id, cancellationToken);

            if (role is null)
                return RoleNotFound(id);

            return Ok(Success("Tenant role updated successfully.", role));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    // PATCH /api/TenantRole/5/status?isActive=false
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromQuery, BindRequired] bool isActive,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return InvalidRoleId();

        try
        {
            var updated = await _service.UpdateStatusAsync(
                id, isActive, cancellationToken);

            if (!updated)
                return RoleNotFound(id);

            return Ok(Success(
                "Tenant role status updated successfully.",
                new { tenantRoleId = id, isActive }));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    // DELETE /api/TenantRole/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return InvalidRoleId();

        try
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return RoleNotFound(id);

            return Ok(Success("Tenant role deleted successfully.", true));
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HandleError(ex);
        }
    }

    private IActionResult InvalidRoleId()
    {
        return BadRequest(Failure("Tenant role ID must be greater than zero."));
    }

    private IActionResult RoleNotFound(int id)
    {
        return NotFound(Failure($"Tenant role {id} was not found."));
    }

    private IActionResult HandleError(Exception exception)
    {
        // The service throws this if the token lacks a valid tenant or user ID.
        if (exception is UnauthorizedAccessException)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                Failure("A valid tenant and user ID are required."));
        }

        var traceId = HttpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "TenantRole API failed for {Method} {Path}. Trace ID {TraceId}",
            Request.Method,
            Request.Path,
            traceId);

        if (_environment.IsDevelopment())
        {
            var cause = exception.GetBaseException();

            return StatusCode(
                StatusCodes.Status500InternalServerError,
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