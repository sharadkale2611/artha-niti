using ArthaNiti.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Interfaces
{

    public interface IPermissionService
    {
        Task<List<PermissionResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<PermissionResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<PermissionResponseDto> CreateAsync(
            PermissionCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id,
            PermissionUpdateDto dto,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}

