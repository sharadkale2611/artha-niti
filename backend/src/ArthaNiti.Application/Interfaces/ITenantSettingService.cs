
using ArthaNiti.Application.TenantSettings.DTOs;

namespace ArthaNiti.Application.TenantSettings.Interfaces;

public interface ITenantSettingService
{
    Task<TenantSettingResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantSettingCreateDto dto);

    Task<List<TenantSettingResponseDto>> GetAllAsync(int tenantId);

    Task<TenantSettingResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantSettingId);

    Task<TenantSettingResponseDto?> UpdateAsync(
        int tenantId,
        int tenantSettingId,
        string currentUserId,
        TenantSettingUpdateDto dto);

    Task<bool> DeleteAsync(
        int tenantId,
        int tenantSettingId,
        string currentUserId);
}
