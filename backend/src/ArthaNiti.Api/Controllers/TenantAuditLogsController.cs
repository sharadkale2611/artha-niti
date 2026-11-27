using System.Security.Claims;
using ArthaNiti.Application.TenantAuditLogs.DTOs;
using ArthaNiti.Application.TenantAuditLogs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TenantAuditLogsController : ControllerBase
{
    private readonly ITenantAuditLogService _auditLogService;

    public TenantAuditLogsController(
        ITenantAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantAuditLogCreateDto dto)
    {
        int tenantId = GetCurrentTenantId();

        string? currentUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        string? ipAddress =
            HttpContext.Connection.RemoteIpAddress?.ToString();

        string? userAgent =
            Request.Headers["User-Agent"].ToString();

        string? correlationId =
            Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? HttpContext.TraceIdentifier;

        var result = await _auditLogService.CreateAsync(
            tenantId,
            currentUserId,
            ipAddress,
            userAgent,
            correlationId,
            dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.TenantAuditLogId },
            result);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        int tenantId = GetCurrentTenantId();

        var result = await _auditLogService.GetAllAsync(tenantId);

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        int tenantId = GetCurrentTenantId();

        var result = await _auditLogService.GetByIdAsync(
            tenantId,
            id);

        if (result == null)
            return NotFound(new
            {
                message = "Audit log not found."
            });

        return Ok(result);
    }

    // GET TENANT ID FROM JWT
    private int GetCurrentTenantId()
    {
        var tenantIdClaim = User.FindFirstValue("TenantId");

        if (!int.TryParse(tenantIdClaim, out int tenantId) ||
            tenantId <= 0)
        {
            throw new UnauthorizedAccessException(
                "Valid TenantId not found in token.");
        }

        return tenantId;
    }
}