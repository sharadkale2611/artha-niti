using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TenantDomainsController : ControllerBase
    {
        private readonly ITenantDomainService _service;
        private readonly ILogger<TenantDomainsController> _logger;

        public TenantDomainsController(
            ITenantDomainService service,
            ILogger<TenantDomainsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST: api/TenantDomains
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TenantDomainCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);

                _logger.LogInformation(
                    "Created tenant domain {TenantDomainId} for tenant {TenantId}.",
                    result.TenantDomainId,
                    result.TenantId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.TenantDomainId },
                    result);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return HandleError(ex);
            }
        }

        // GET: api/TenantDomains
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();

                _logger.LogDebug(
                    "Retrieved {Count} domains for tenant {TenantId}.",
                    result.Count,
                    User.FindFirst("TenantId")?.Value);

                return Ok(result);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return HandleError(ex);
            }
        }

        // GET: api/TenantDomains/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Tenant domain ID must be greater than zero.");

            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result is null)
                    return NotFound("Tenant domain not found.");

                _logger.LogDebug(
                    "Retrieved tenant domain {TenantDomainId} for tenant {TenantId}.",
                    id,
                    result.TenantId);

                return Ok(result);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return HandleError(ex);
            }
        }

        // PUT: api/TenantDomains/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] TenantDomainUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest("Tenant domain ID must be greater than zero.");

            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (result is null)
                    return NotFound("Tenant domain not found.");

                _logger.LogInformation(
                    "Updated tenant domain {TenantDomainId} for tenant {TenantId}.",
                    id,
                    result.TenantId);

                return Ok(result);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return HandleError(ex);
            }
        }

        // DELETE: api/TenantDomains/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Tenant domain ID must be greater than zero.");

            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound("Tenant domain not found.");

                _logger.LogInformation(
                    "Soft-deleted tenant domain {TenantDomainId} for tenant {TenantId}.",
                    id,
                    User.FindFirst("TenantId")?.Value);

                return Ok(new
                {
                    success = true,
                    message = "Tenant domain deleted successfully."
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return HandleError(ex);
            }
        }

        private IActionResult HandleError(Exception exception)
        {
            var traceId = HttpContext.TraceIdentifier;

            if (exception is UnauthorizedAccessException)
            {
                _logger.LogWarning(
                    "Tenant domain access denied for {Method} {Path}. " +
                    "Reason: {Reason}. Trace ID {TraceId}.",
                    Request.Method,
                    Request.Path,
                    exception.Message,
                    traceId);

                return Forbid();
            }

            _logger.LogError(
                exception,
                "Tenant domain request failed for {Method} {Path}. Trace ID {TraceId}.",
                Request.Method,
                Request.Path,
                traceId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    traceId
                });
        }
    }
}
