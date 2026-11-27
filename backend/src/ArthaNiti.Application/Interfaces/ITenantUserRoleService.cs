
using ArthaNiti.Application.TenantUserRoles.DTOs;

namespace ArthaNiti.Application.TenantUserRoles.Interfaces;

public interface ITenantUserRoleService
{
    Task<TenantUserRoleResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantUserRoleCreateDto dto);

    Task<List<TenantUserRoleResponseDto>> GetAllAsync(
        int tenantId);

    Task<TenantUserRoleResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantUserRoleId);

    Task<TenantUserRoleResponseDto?> UpdateAsync(
        int tenantId,
        int tenantUserRoleId,
        string currentUserId,
        TenantUserRoleUpdateDto dto);

    Task<bool> DeleteAsync(
        int tenantId,
        int tenantUserRoleId,
        string currentUserId);
}
