
using ArthaNiti.Application.TenantUserRoles.DTOs;
using ArthaNiti.Application.TenantUserRoles.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArthaNiti.Infrastructure.Services;

public class TenantUserRoleService : ITenantUserRoleService
{
    private readonly ArthaNitiDbContext _context;

    public TenantUserRoleService(ArthaNitiDbContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<TenantUserRoleResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantUserRoleCreateDto dto)
    {
        // Check TenantUser exists
        bool tenantUserExists = await _context.Set<TenantUser>()
            .AnyAsync(x =>
                x.TenantUserId == dto.TenantUserId &&
                x.TenantId == tenantId &&
                !x.IsDeleted &&
                x.IsActive);

        if (!tenantUserExists)
        {
            throw new InvalidOperationException(
                "Tenant user not found or inactive.");
        }

        // Check duplicate role assignment
        bool alreadyAssigned = await _context.Set<TenantUserRole>()
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.TenantUserId == dto.TenantUserId &&
                x.TenantRoleId == dto.TenantRoleId);

        if (alreadyAssigned)
        {
            throw new InvalidOperationException(
                "This role is already assigned to the user.");
        }

        var entity = new TenantUserRole
        {
            TenantId = tenantId,
            TenantUserId = dto.TenantUserId,
            TenantRoleId = dto.TenantRoleId,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.Set<TenantUserRole>().Add(entity);

        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    // GET ALL
    public async Task<List<TenantUserRoleResponseDto>> GetAllAsync(
        int tenantId)
    {
        var roles = await _context.Set<TenantUserRole>()
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.TenantUserRoleId)
            .ToListAsync();

        return roles.Select(MapToDto).ToList();
    }

    // GET BY ID
    public async Task<TenantUserRoleResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantUserRoleId)
    {
        var role = await _context.Set<TenantUserRole>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantUserRoleId == tenantUserRoleId &&
                x.TenantId == tenantId &&
                !x.IsDeleted);

        return role == null ? null : MapToDto(role);
    }

    // UPDATE
    public async Task<TenantUserRoleResponseDto?> UpdateAsync(
        int tenantId,
        int tenantUserRoleId,
        string currentUserId,
        TenantUserRoleUpdateDto dto)
    {
        var role = await _context.Set<TenantUserRole>()
            .FirstOrDefaultAsync(x =>
                x.TenantUserRoleId == tenantUserRoleId &&
                x.TenantId == tenantId &&
                !x.IsDeleted);

        if (role == null)
            return null;

        role.IsActive = dto.IsActive;
        role.UpdatedAt = DateTime.Now;
        role.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return MapToDto(role);
    }

    // SOFT DELETE
    public async Task<bool> DeleteAsync(
        int tenantId,
        int tenantUserRoleId,
        string currentUserId)
    {
        var role = await _context.Set<TenantUserRole>()
            .FirstOrDefaultAsync(x =>
                x.TenantUserRoleId == tenantUserRoleId &&
                x.TenantId == tenantId &&
                !x.IsDeleted);

        if (role == null)
            return false;

        role.IsDeleted = true;
        role.IsActive = false;
        role.UpdatedAt = DateTime.Now;
        role.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return true;
    }

    // ENTITY TO DTO MAPPING
    private static TenantUserRoleResponseDto MapToDto(
        TenantUserRole entity)
    {
        return new TenantUserRoleResponseDto
        {
            TenantUserRoleId = entity.TenantUserRoleId,
            TenantId = entity.TenantId,
            TenantUserId = entity.TenantUserId,
            TenantRoleId = entity.TenantRoleId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }
}
