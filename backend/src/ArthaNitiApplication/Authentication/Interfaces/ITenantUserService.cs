using ArthaNiti.Application.Authentication.DTOs;

namespace ArthaNiti.Application.Authentication.Interfaces
{
    public interface ITenantUserService
    {
        Task<List<TenantUserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<TenantUserResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<TenantUserResponseDto> CreateAsync(
            TenantUserCreateDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id,
            TenantUserUpdateDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}