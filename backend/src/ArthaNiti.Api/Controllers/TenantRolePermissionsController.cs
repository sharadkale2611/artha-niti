
using ArthaNiti.Application.TenantRolePermissions.DTOs;
using ArthaNiti.Application.TenantRolePermissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArthaNiti.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TenantRolePermissionsController : ControllerBase
{
    private readonly ITenantRolePermissionService _service;

    public TenantRolePermissionsController(
        ITenantRolePermissionService service)
    {
        _service = service;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantRolePermissionCreateDto dto)
    {
        var tenantId = GetTenantId();
        var currentUserId = GetCurrentUserId();

        var result = await _service.CreateAsync(
            tenantId,
            currentUserId,
            dto);

        return CreatedAtAction(
            nameof(GetById),
            new { tenantRolePermissionId = result.TenantRolePermissionId },
            result);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tenantId = GetTenantId();

        var result = await _service.GetAllAsync(tenantId);

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{tenantRolePermissionId:int}")]
    public async Task<IActionResult> GetById(int tenantRolePermissionId)
    {
        var tenantId = GetTenantId();

        var result = await _service.GetByIdAsync(
            tenantId,
            tenantRolePermissionId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Tenant role permission not found."
            });
        }

        return Ok(result);
    }

    // UPDATE
    [HttpPut("{tenantRolePermissionId:int}")]
    public async Task<IActionResult> Update(
        int tenantRolePermissionId,
        [FromBody] TenantRolePermissionUpdateDto dto)
    {
        var tenantId = GetTenantId();
        var currentUserId = GetCurrentUserId();

        var result = await _service.UpdateAsync(
            tenantId,
            tenantRolePermissionId,
            currentUserId,
            dto);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Tenant role permission not found."
            });
        }

        return Ok(result);
    }

    // DELETE
    [HttpDelete("{tenantRolePermissionId:int}")]
    public async Task<IActionResult> Delete(int tenantRolePermissionId)
    {
        var tenantId = GetTenantId();
        var currentUserId = GetCurrentUserId();

        var result = await _service.DeleteAsync(
            tenantId,
            tenantRolePermissionId,
            currentUserId);

        if (!result)
        {
            return NotFound(new
            {
                message = "Tenant role permission not found."
            });
        }

        return Ok(new
        {
            message = "Tenant role permission deleted successfully."
        });
    }

    // TENANT ID FROM JWT
    private int GetTenantId()
    {
        var tenantClaim = User.FindFirst("TenantId")?.Value;

        if (!int.TryParse(tenantClaim, out var tenantId))
        {
            throw new UnauthorizedAccessException(
                "Invalid or missing TenantId claim.");
        }

        return tenantId;
    }

    // CURRENT USER ID FROM JWT
    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException(
                "UserId claim not found.");
    }
}
