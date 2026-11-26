using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArthaNiti.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly ArthaNitiDbContext _context;

    public TenantService(ArthaNitiDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var tenants = await _context.Tenants
            .AsNoTracking()
            .OrderBy(t => t.TenantId)
            .ToListAsync(cancellationToken);

        return tenants.Select(MapToResponse).ToList();
    }

    public async Task<TenantResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TenantId == id, cancellationToken);

        return tenant is null ? null : MapToResponse(tenant);
    }

    public async Task<TenantResponseDto> CreateAsync(
        TenantCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            TenantCode = dto.TenantCode,
            TenantName = dto.TenantName,
            Slug = dto.Slug,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Status = dto.Status,
            IsActive = dto.IsActive,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponse(tenant);
    }

    public async Task<bool> UpdateAsync(
        int id,
        TenantUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.TenantId == id, cancellationToken);

        if (tenant is null)
            return false;

        tenant.TenantCode = dto.TenantCode;
        tenant.TenantName = dto.TenantName;
        tenant.Slug = dto.Slug;
        tenant.Email = dto.Email;
        tenant.PhoneNumber = dto.PhoneNumber;
        tenant.Status = (int?)dto.Status ?? tenant.Status;
        tenant.IsActive = (bool?)dto.IsActive ?? tenant.IsActive;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var updatedRows = await _context.Tenants
            .Where(t => t.TenantId == id && t.IsDeleted == false)
            .ExecuteUpdateAsync(set => set
                .SetProperty(t => t.IsDeleted, true)
                .SetProperty(t => t.IsActive, false)
                .SetProperty(t => t.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        return updatedRows > 0;
    }

    private static TenantResponseDto MapToResponse(Tenant tenant)
    {
        return new TenantResponseDto
        {
            TenantId = tenant.TenantId,
            TenantCode = tenant.TenantCode,
            TenantName = tenant.TenantName,
            Slug = tenant.Slug,
            Email = tenant.Email,
            PhoneNumber = tenant.PhoneNumber,
            Status = RequiredValue(tenant.Status, nameof(Tenant.Status), tenant.TenantId),
            IsActive = RequiredValue(tenant.IsActive, nameof(Tenant.IsActive), tenant.TenantId),
            IsDeleted = RequiredValue(tenant.IsDeleted, nameof(Tenant.IsDeleted), tenant.TenantId),
            CreatedAt = RequiredValue(tenant.CreatedAt, nameof(Tenant.CreatedAt), tenant.TenantId),
            CreatedBy = tenant.CreatedBy,
            UpdatedAt = tenant.UpdatedAt,
            UpdatedBy = tenant.UpdatedBy
        };
    }

    private static T RequiredValue<T>(T? value, string field, int tenantId)
        where T : struct
    {
        return value ?? throw new InvalidOperationException(
            $"TenantId {tenantId} has NULL in required field {field}.");
    }
}