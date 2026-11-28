using ArthaNiti.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Interfaces
{
    public interface ITenantDomainService
    {
        Task<TenantDomainResponseDto> CreateAsync(TenantDomainCreateDto dto);

        Task<List<TenantDomainResponseDto>> GetAllAsync();

        Task<TenantDomainResponseDto?> GetByIdAsync(int tenantDomainId);

        Task<TenantDomainResponseDto?> UpdateAsync(
            int tenantDomainId,
            TenantDomainUpdateDto dto);

        Task<bool> DeleteAsync(int tenantDomainId);
    }
}
