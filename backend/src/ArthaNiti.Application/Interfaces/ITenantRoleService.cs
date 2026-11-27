using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArthaNiti.Application.DTOs;

namespace ArthaNiti.Application.Interfaces;

public interface ITenantRoleService
{
    Task<List<TenantRoleResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TenantRoleResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<TenantRoleResponseDto> CreateAsync(
        TenantRoleCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        TenantRoleUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}