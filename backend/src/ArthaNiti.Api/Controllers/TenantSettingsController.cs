
using System.Security.Claims;
using ArthaNiti.Application.TenantSettings.DTOs;
using ArthaNiti.Application.TenantSettings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantSettingsController : ControllerBase
{
    private readonly ITenantSettingService _service;

    public TenantSettingsController(ITenantSettingService service)
    {
        _service = service;
    }

    // POST: api/TenantSettings
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TenantSettingCreateDto dto)
    {
        if (!TryGetTenantId(out int tenantId))
            return Unauthorized("TenantId claim not found.");

        string currentUserId = GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized("User ID claim not found.");

        try
        {
            var result = await _service.CreateAsync(
                tenantId, currentUserId, dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.TenantSettingId },
                result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // GET: api/TenantSettings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetTenantId(out int tenantId))
            return Unauthorized("TenantId claim not found.");

        var result = await _service.GetAllAsync(tenantId);

        return Ok(result);
    }

    // GET: api/TenantSettings/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!TryGetTenantId(out int tenantId))
            return Unauthorized("TenantId claim not found.");

        var result = await _service.GetByIdAsync(tenantId, id);

        if (result == null)
            return NotFound("Tenant setting not found.");

        return Ok(result);
    }

    // PUT: api/TenantSettings/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] TenantSettingUpdateDto dto)
    {
        if (!TryGetTenantId(out int tenantId))
            return Unauthorized("TenantId claim not found.");

        string currentUserId = GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized("User ID claim not found.");

        var result = await _service.UpdateAsync(
            tenantId, id, currentUserId, dto);

        if (result == null)
            return NotFound("Tenant setting not found.");

        return Ok(result);
    }

    // DELETE: api/TenantSettings/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetTenantId(out int tenantId))
            return Unauthorized("TenantId claim not found.");

        string currentUserId = GetCurrentUserId();

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized("User ID claim not found.");

        bool deleted = await _service.DeleteAsync(
            tenantId, id, currentUserId);

        if (!deleted)
            return NotFound("Tenant setting not found.");

        return Ok(new
        {
            success = true,
            message = "Tenant setting deleted successfully."
        });
    }

    private bool TryGetTenantId(out int tenantId)
    {
        tenantId = 0;

        var claim = User.FindFirst("TenantId")?.Value;

        return int.TryParse(claim, out tenantId) && tenantId > 0;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? string.Empty;
    }
}
