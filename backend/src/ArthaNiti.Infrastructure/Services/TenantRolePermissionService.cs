
using ArthaNiti.Application.TenantRolePermissions.DTOs;
using ArthaNiti.Application.TenantRolePermissions.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArthaNiti.Infrastructure.Services;

public class TenantRolePermissionService : ITenantRolePermissionService
{
    private readonly ArthaNitiDbContext _context;
    private readonly ILogger<TenantRolePermissionService> _logger;

    public TenantRolePermissionService(
        ArthaNitiDbContext context,
        ILogger<TenantRolePermissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // CREATE
    public async Task<TenantRolePermissionResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantRolePermissionCreateDto dto)
    {
        var existing = await _context.TenantRolePermissions
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantRoleId == dto.TenantRoleId &&
                x.PermissionId == dto.PermissionId);

        if (existing != null)
        {
            if (!existing.IsDeleted && existing.IsActive)
            {
                _logger.LogWarning(
                    "Duplicate permission assignment. TenantId: {TenantId}, RoleId: {RoleId}, PermissionId: {PermissionId}",
                    tenantId,
                    dto.TenantRoleId,
                    dto.PermissionId);

                throw new InvalidOperationException(
                    "This permission is already assigned to the role.");
            }

            existing.IsDeleted = false;
            existing.IsActive = true;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = currentUserId;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Permission assignment reactivated. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
                existing.TenantRolePermissionId,
                tenantId);

            return MapToResponse(existing);
        }

        var assignment = new TenantRolePermission
        {
            TenantId = tenantId,
            TenantRoleId = dto.TenantRoleId,
            PermissionId = dto.PermissionId,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.TenantRolePermissions.Add(assignment);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Permission assignment created. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
            assignment.TenantRolePermissionId,
            tenantId);

        return MapToResponse(assignment);
    }

    // GET ALL
    public async Task<List<TenantRolePermissionResponseDto>> GetAllAsync(
        int tenantId)
    {
        var assignments = await _context.TenantRolePermissions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.TenantRolePermissionId)
            .ToListAsync();

        _logger.LogInformation(
            "Permission assignments retrieved. TenantId: {TenantId}, Count: {Count}",
            tenantId,
            assignments.Count);

        return assignments.Select(MapToResponse).ToList();
    }

    // GET BY ID
    public async Task<TenantRolePermissionResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantRolePermissionId)
    {
        var assignment = await _context.TenantRolePermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantRolePermissionId == tenantRolePermissionId &&
                !x.IsDeleted);

        if (assignment == null)
        {
            _logger.LogWarning(
                "Permission assignment not found. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
                tenantRolePermissionId,
                tenantId);

            return null;
        }

        return MapToResponse(assignment);
    }

    // UPDATE
    public async Task<TenantRolePermissionResponseDto?> UpdateAsync(
        int tenantId,
        int tenantRolePermissionId,
        string currentUserId,
        TenantRolePermissionUpdateDto dto)
    {
        var assignment = await _context.TenantRolePermissions
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantRolePermissionId == tenantRolePermissionId &&
                !x.IsDeleted);

        if (assignment == null)
        {
            _logger.LogWarning(
                "Permission assignment update failed. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
                tenantRolePermissionId,
                tenantId);

            return null;
        }

        assignment.IsActive = dto.IsActive;
        assignment.UpdatedAt = DateTime.Now;
        assignment.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Permission assignment updated. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
            tenantRolePermissionId,
            tenantId);

        return MapToResponse(assignment);
    }

    // DELETE - SOFT DELETE
    public async Task<bool> DeleteAsync(
        int tenantId,
        int tenantRolePermissionId,
        string currentUserId)
    {
        var assignment = await _context.TenantRolePermissions
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantRolePermissionId == tenantRolePermissionId &&
                !x.IsDeleted);

        if (assignment == null)
        {
            _logger.LogWarning(
                "Permission assignment delete failed. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
                tenantRolePermissionId,
                tenantId);

            return false;
        }

        assignment.IsDeleted = true;
        assignment.IsActive = false;
        assignment.UpdatedAt = DateTime.Now;
        assignment.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Permission assignment soft deleted. AssignmentId: {AssignmentId}, TenantId: {TenantId}",
            tenantRolePermissionId,
            tenantId);

        return true;
    }

    // RESPONSE MAPPING
    private static TenantRolePermissionResponseDto MapToResponse(
        TenantRolePermission entity)
    {
        return new TenantRolePermissionResponseDto
        {
            TenantRolePermissionId = entity.TenantRolePermissionId,
            TenantId = entity.TenantId,
            TenantRoleId = entity.TenantRoleId,
            PermissionId = entity.PermissionId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }
}
