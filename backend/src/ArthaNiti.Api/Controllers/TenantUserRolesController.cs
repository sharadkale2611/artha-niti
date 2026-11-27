
using ArthaNiti.Application.TenantUserRoles.DTOs;
using ArthaNiti.Application.TenantUserRoles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArthaNiti.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantUserRolesController : ControllerBase
{
    private readonly ITenantUserRoleService _service;

    public TenantUserRolesController(
        ITenantUserRoleService service)
    {
        _service = service;
    }

    private bool TryGetCurrentContext(
        out int tenantId,
        out string currentUserId)
    {
        tenantId = 0;
        currentUserId = string.Empty;

        var tenantClaim = User.FindFirst("TenantId")?.Value;

        var userClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (!int.TryParse(tenantClaim, out tenantId))
            return false;

        if (string.IsNullOrWhiteSpace(userClaim))
            return false;

        currentUserId = userClaim;

        return true;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantUserRoleCreateDto dto)
    {
        if (!TryGetCurrentContext(
            out int tenantId, out string userId))
            return Unauthorized();

        try
        {
            var result = await _service.CreateAsync(
                tenantId, userId, dto);

            return Ok(new
            {
                success = true,
                message = "Role assigned successfully.",
                data = result,
                errors = (object?)null
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message,
                data = (object?)null,
                errors = new[] { ex.Message }
            });
        }
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetCurrentContext(out int tenantId, out _))
            return Unauthorized();

        var result = await _service.GetAllAsync(tenantId);

        return Ok(new
        {
            success = true,
            message = "Tenant user roles fetched successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // GET BY ID
    [HttpGet("{tenantUserRoleId:int}")]
    public async Task<IActionResult> GetById(
        int tenantUserRoleId)
    {
        if (!TryGetCurrentContext(out int tenantId, out _))
            return Unauthorized();

        var result = await _service.GetByIdAsync(
            tenantId, tenantUserRoleId);

        if (result == null)
            return NotFound(new
            {
                success = false,
                message = "Tenant user role not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user role fetched successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // UPDATE
    [HttpPut("{tenantUserRoleId:int}")]
    public async Task<IActionResult> Update(
        int tenantUserRoleId,
        [FromBody] TenantUserRoleUpdateDto dto)
    {
        if (!TryGetCurrentContext(
            out int tenantId, out string userId))
            return Unauthorized();

        var result = await _service.UpdateAsync(
            tenantId,
            tenantUserRoleId,
            userId,
            dto);

        if (result == null)
            return NotFound(new
            {
                success = false,
                message = "Tenant user role not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user role updated successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // DELETE - SOFT DELETE
    [HttpDelete("{tenantUserRoleId:int}")]
    public async Task<IActionResult> Delete(
        int tenantUserRoleId)
    {
        if (!TryGetCurrentContext(
            out int tenantId, out string userId))
            return Unauthorized();

        var result = await _service.DeleteAsync(
            tenantId,
            tenantUserRoleId,
            userId);

        if (!result)
            return NotFound(new
            {
                success = false,
                message = "Tenant user role not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user role deleted successfully.",
            data = (object?)null,
            errors = (object?)null
        });
    }
}
