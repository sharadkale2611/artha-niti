
using ArthaNiti.Application.TenantRolePermissions.DTOs;

namespace ArthaNiti.Application.TenantRolePermissions.Interfaces;

public interface ITenantRolePermissionService
{
    Task<TenantRolePermissionResponseDto> CreateAsync(
        int tenantId,
        string currentUserId,
        TenantRolePermissionCreateDto dto);

    Task<List<TenantRolePermissionResponseDto>> GetAllAsync(
        int tenantId);

    Task<TenantRolePermissionResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantRolePermissionId);

    Task<TenantRolePermissionResponseDto?> UpdateAsync(
        int tenantId,
        int tenantRolePermissionId,
        string currentUserId,
        TenantRolePermissionUpdateDto dto);

    Task<bool> DeleteAsync(
        int tenantId,
        int tenantRolePermissionId,
        string currentUserId);
}
