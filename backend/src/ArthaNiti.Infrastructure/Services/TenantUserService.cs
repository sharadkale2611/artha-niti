
using ArthaNiti.Application.TenantUsers.DTOs;
using ArthaNiti.Application.TenantUsers.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArthaNiti.Infrastructure.Services;

public class TenantUserService : ITenantUserService
{
    private readonly ArthaNitiDbContext _context;

    public TenantUserService(ArthaNitiDbContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<TenantUserResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantUserCreateDto dto)
    {
        bool exists = await _context.Set<TenantUser>()
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.UserId == dto.UserId);

        if (exists)
            throw new InvalidOperationException(
                "User already exists in this tenant.");

        var entity = new TenantUser
        {
            TenantId = tenantId,
            UserId = dto.UserId,
            IsPrimary = dto.IsPrimary,
            Status = dto.Status,
            JoinedAt = DateTime.Now,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.Set<TenantUser>().Add(entity);

        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    // GET ALL
    public async Task<List<TenantUserResponseDto>> GetAllAsync(
        int tenantId)
    {
        var users = await _context.Set<TenantUser>()
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.TenantUserId)
            .ToListAsync();

        return users.Select(MapToDto).ToList();
    }

    // GET BY ID
    public async Task<TenantUserResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantUserId)
    {
        var user = await _context.Set<TenantUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantUserId == tenantUserId &&
                !x.IsDeleted);

        return user == null ? null : MapToDto(user);
    }

    // UPDATE
    public async Task<TenantUserResponseDto?> UpdateAsync(
        int tenantId,
        int tenantUserId,
        string currentUserId,
        TenantUserUpdateDto dto)
    {
        var user = await _context.Set<TenantUser>()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantUserId == tenantUserId &&
                !x.IsDeleted);

        if (user == null)
            return null;

        user.IsPrimary = dto.IsPrimary;
        user.Status = dto.Status;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    // SOFT DELETE
    public async Task<bool> DeleteAsync(
        int tenantId,
        int tenantUserId,
        string currentUserId)
    {
        var user = await _context.Set<TenantUser>()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantUserId == tenantUserId &&
                !x.IsDeleted);

        if (user == null)
            return false;

        user.IsDeleted = true;
        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return true;
    }

    // ENTITY TO DTO MAPPING
    private static TenantUserResponseDto MapToDto(
        TenantUser user)
    {
        return new TenantUserResponseDto
        {
            TenantUserId = user.TenantUserId,
            TenantId = user.TenantId,
            UserId = user.UserId,
            IsPrimary = user.IsPrimary,
            Status = user.Status,
            JoinedAt = user.JoinedAt,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            CreatedBy = user.CreatedBy,
            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy
        };
    }
}
