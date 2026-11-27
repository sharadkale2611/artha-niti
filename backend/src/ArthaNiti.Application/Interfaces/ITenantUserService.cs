
using ArthaNiti.Application.TenantUsers.DTOs;

namespace ArthaNiti.Application.TenantUsers.Interfaces;

public interface ITenantUserService
{
    Task<TenantUserResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantUserCreateDto dto);

    Task<List<TenantUserResponseDto>> GetAllAsync(
        int tenantId);

    Task<TenantUserResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantUserId);

    Task<TenantUserResponseDto?> UpdateAsync(
        int tenantId,
        int tenantUserId,
        string currentUserId,
        TenantUserUpdateDto dto);

    Task<bool> DeleteAsync(
        int tenantId,
        int tenantUserId,
        string currentUserId);
}
