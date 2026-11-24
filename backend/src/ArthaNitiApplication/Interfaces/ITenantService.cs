using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArthaNiti.Application.DTOs;

namespace ArthaNiti.Application.Interfaces;

public interface ITenantService
{
    Task<List<TenantResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TenantResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<TenantResponseDto> CreateAsync(
        TenantCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        TenantUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}