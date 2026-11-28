using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entites;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ArthaNiti.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ArthaNitiDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionService(
        ArthaNitiDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private IQueryable<Permission> Permissions =>
        _context.Set<Permission>().Where(x => x.IsDeleted == false);

    // GET ALL
    public async Task<List<PermissionResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await Permissions
            .AsNoTracking()
            .OrderBy(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToResponse).ToList();
    }

    // GET BY ID
    public async Task<PermissionResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PermissionId == id,
                cancellationToken);

        return entity is null ? null : MapToResponse(entity);
    }

    // CREATE
    public async Task<PermissionResponseDto> CreateAsync(
        PermissionCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var entity = new Permission
        {
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            ModuleCode = dto.ModuleCode.Trim(),
            PluginCode = dto.PluginCode?.Trim(),
            PermissionScope = (int?)dto.PermissionScope ?? 2,
            IsSystemPermission = (bool?)dto.IsSystemPermission ?? false,
            IsActive = (bool?)dto.IsActive ?? true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = GetCurrentUserId()
        };

        _context.Set<Permission>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(entity);
    }

    // UPDATE
    public async Task<bool> UpdateAsync(
        int id,
        PermissionUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var code = dto.Code.Trim();
        var name = dto.Name.Trim();
        var description = dto.Description?.Trim();
        var moduleCode = dto.ModuleCode.Trim();
        var pluginCode = dto.PluginCode?.Trim();
        var updatedAt = DateTime.UtcNow;
        var updatedBy = GetCurrentUserId();

        var affectedRows = await Permissions
            .Where(x => x.PermissionId == id)
            .ExecuteUpdateAsync(setters =>
            {
                setters.SetProperty(x => x.Code, code);
                setters.SetProperty(x => x.Name, name);
                setters.SetProperty(x => x.Description, description);
                setters.SetProperty(x => x.ModuleCode, moduleCode);
                setters.SetProperty(x => x.PluginCode, pluginCode);
                setters.SetProperty(x => x.UpdatedAt, updatedAt);
                setters.SetProperty(x => x.UpdatedBy, updatedBy);

                if (dto.PermissionScope is int scope)
                    setters.SetProperty(x => x.PermissionScope, scope);

                if (dto.IsSystemPermission is bool isSystemPermission)
                    setters.SetProperty(x => x.IsSystemPermission, isSystemPermission);

                if (dto.IsActive is bool isActive)
                    setters.SetProperty(x => x.IsActive, isActive);
            }, cancellationToken);

        return affectedRows > 0;
    }

    // SOFT DELETE
    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var updatedAt = DateTime.UtcNow;
        var updatedBy = GetCurrentUserId();

        var affectedRows = await Permissions
            .Where(x => x.PermissionId == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsDeleted, true)
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.UpdatedAt, updatedAt)
                .SetProperty(x => x.UpdatedBy, updatedBy),
                cancellationToken);

        return affectedRows > 0;
    }

    // UPDATE ACTIVE STATUS
    public async Task<bool> UpdateStatusAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var updatedAt = DateTime.UtcNow;
        var updatedBy = GetCurrentUserId();

        var affectedRows = await Permissions
            .Where(x => x.PermissionId == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, isActive)
                .SetProperty(x => x.UpdatedAt, updatedAt)
                .SetProperty(x => x.UpdatedBy, updatedBy),
                cancellationToken);

        return affectedRows > 0;
    }

    private static PermissionResponseDto MapToResponse(Permission entity)
    {
        return new PermissionResponseDto
        {
            PermissionId = entity.PermissionId,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            ModuleCode = entity.ModuleCode,
            PluginCode = entity.PluginCode,
            PermissionScope = (int)entity.PermissionScope!,
            IsSystemPermission = entity.IsSystemPermission == true,
            IsActive = entity.IsActive == true,
            IsDeleted = entity.IsDeleted == true,
            CreatedAt = (DateTime)entity.CreatedAt!,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }

    private string? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst("sub")?.Value;
    }
}