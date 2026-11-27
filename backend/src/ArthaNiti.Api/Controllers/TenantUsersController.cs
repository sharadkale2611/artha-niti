
using ArthaNiti.Application.TenantUsers.DTOs;
using ArthaNiti.Application.TenantUsers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArthaNiti.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantUsersController : ControllerBase
{
    private readonly ITenantUserService _service;

    public TenantUsersController(ITenantUserService service)
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

        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
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
        [FromBody] TenantUserCreateDto dto)
    {
        if (!TryGetCurrentContext(out int tenantId, out string userId))
            return Unauthorized(new
            {
                success = false,
                message = "Invalid Tenant or User claims.",
                data = (object?)null,
                errors = new[] { "TenantId or UserId claim missing." }
            });

        try
        {
            var result = await _service.CreateAsync(
                tenantId, userId, dto);

            return Ok(new
            {
                success = true,
                message = "Tenant user created successfully.",
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
            message = "Tenant users fetched successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // GET BY ID
    [HttpGet("{tenantUserId:int}")]
    public async Task<IActionResult> GetById(int tenantUserId)
    {
        if (!TryGetCurrentContext(out int tenantId, out _))
            return Unauthorized();

        var result = await _service.GetByIdAsync(
            tenantId, tenantUserId);

        if (result == null)
            return NotFound(new
            {
                success = false,
                message = "Tenant user not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user fetched successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // UPDATE
    [HttpPut("{tenantUserId:int}")]
    public async Task<IActionResult> Update(
        int tenantUserId,
        [FromBody] TenantUserUpdateDto dto)
    {
        if (!TryGetCurrentContext(out int tenantId, out string userId))
            return Unauthorized();

        var result = await _service.UpdateAsync(
            tenantId, tenantUserId, userId, dto);

        if (result == null)
            return NotFound(new
            {
                success = false,
                message = "Tenant user not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user updated successfully.",
            data = result,
            errors = (object?)null
        });
    }

    // DELETE (SOFT DELETE)
    [HttpDelete("{tenantUserId:int}")]
    public async Task<IActionResult> Delete(int tenantUserId)
    {
        if (!TryGetCurrentContext(out int tenantId, out string userId))
            return Unauthorized();

        var result = await _service.DeleteAsync(
            tenantId, tenantUserId, userId);

        if (!result)
            return NotFound(new
            {
                success = false,
                message = "Tenant user not found.",
                data = (object?)null,
                errors = (object?)null
            });

        return Ok(new
        {
            success = true,
            message = "Tenant user deleted successfully.",
            data = (object?)null,
            errors = (object?)null
        });
    }
}
