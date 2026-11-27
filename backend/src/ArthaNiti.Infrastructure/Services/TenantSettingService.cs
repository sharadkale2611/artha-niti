
using ArthaNiti.Application.TenantSettings.DTOs;
using ArthaNiti.Application.TenantSettings.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArthaNiti.Infrastructure.Services;

public class TenantSettingService : ITenantSettingService
{
    private readonly ArthaNitiDbContext _context;

    public TenantSettingService(ArthaNitiDbContext context)
    {
        _context = context;
    }

    public async Task<TenantSettingResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantSettingCreateDto dto)
    {
        string settingKey = dto.SettingKey.Trim();

        // Check duplicate key in the same tenant.
        // Include deleted records because the SQL unique constraint
        // also includes deleted records.
        bool exists = await _context.TenantSettings.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.SettingKey == settingKey);

        if (exists)
            throw new InvalidOperationException(
                "This setting key already exists for this tenant.");

        var setting = new TenantSetting
        {
            TenantId = tenantId,
            SettingKey = settingKey,
            SettingValue = dto.SettingValue,
            ValueType = dto.ValueType.Trim(),
            Description = dto.Description,
            IsEncrypted = dto.IsEncrypted,
            IsSystemSetting = dto.IsSystemSetting,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.TenantSettings.Add(setting);
        await _context.SaveChangesAsync();

        return MapToResponse(setting);
    }

    public async Task<List<TenantSettingResponseDto>> GetAllAsync(
        int tenantId)
    {
        var settings = await _context.TenantSettings
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderBy(x => x.SettingKey)
            .ToListAsync();

        return settings.Select(MapToResponse).ToList();
    }

    public async Task<TenantSettingResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantSettingId)
    {
        var setting = await _context.TenantSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantSettingId == tenantSettingId &&
                !x.IsDeleted);

        return setting == null ? null : MapToResponse(setting);
    }

    public async Task<TenantSettingResponseDto?> UpdateAsync(
        int tenantId,
        int tenantSettingId,
        string currentUserId,
        TenantSettingUpdateDto dto)
    {
        var setting = await _context.TenantSettings
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantSettingId == tenantSettingId &&
                !x.IsDeleted);

        if (setting == null)
            return null;

        setting.SettingValue = dto.SettingValue;
        setting.ValueType = dto.ValueType.Trim();
        setting.Description = dto.Description;
        setting.IsEncrypted = dto.IsEncrypted;
        setting.IsSystemSetting = dto.IsSystemSetting;
        setting.IsActive = dto.IsActive;
        setting.UpdatedAt = DateTime.Now;
        setting.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return MapToResponse(setting);
    }

    public async Task<bool> DeleteAsync(
        int tenantId,
        int tenantSettingId,
        string currentUserId)
    {
        var setting = await _context.TenantSettings
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantSettingId == tenantSettingId &&
                !x.IsDeleted);

        if (setting == null)
            return false;

        setting.IsDeleted = true;
        setting.IsActive = false;
        setting.UpdatedAt = DateTime.Now;
        setting.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        return true;
    }

    private static TenantSettingResponseDto MapToResponse(
        TenantSetting setting)
    {
        return new TenantSettingResponseDto
        {
            TenantSettingId = setting.TenantSettingId,
            TenantId = setting.TenantId,
            SettingKey = setting.SettingKey,
            SettingValue = setting.SettingValue,
            ValueType = setting.ValueType,
            Description = setting.Description,
            IsEncrypted = setting.IsEncrypted,
            IsSystemSetting = setting.IsSystemSetting,
            IsActive = setting.IsActive,
            CreatedAt = setting.CreatedAt,
            CreatedBy = setting.CreatedBy,
            UpdatedAt = setting.UpdatedAt,
            UpdatedBy = setting.UpdatedBy
        };
    }
}
