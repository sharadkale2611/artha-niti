using ArthaNiti.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Interfaces
{
    public interface IMenuPermissionService
    {
        Task<List<MenuPermissionResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<MenuPermissionResponseDto?> GetByIdAsync(
            int menuPermissionId,
            CancellationToken cancellationToken = default);

        Task<MenuPermissionResponseDto> CreateAsync(
            MenuPermissionCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<MenuPermissionResponseDto?> UpdateAsync(
            int menuPermissionId,
            MenuPermissionUpdateDto dto,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(
            int menuPermissionId,
            bool isActive,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(
            int menuPermissionId,
            CancellationToken cancellationToken = default);
    }
}
