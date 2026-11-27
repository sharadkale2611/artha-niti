using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ArthaNiti.Infrastructure.Services
{
    public class TenantDomainService : ITenantDomainService
    {
        private readonly ArthaNitiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantDomainService(
            ArthaNitiDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TenantDomainResponseDto> CreateAsync(
            TenantDomainCreateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var domain = new TenantDomain
            {
                TenantId = GetCurrentTenantId(),
                DomainName = dto.DomainName.Trim().ToLowerInvariant(),
                DomainType = dto.DomainType,
                IsPrimary = dto.IsPrimary,
                IsVerified = false,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = GetCurrentUserId()
            };

            _context.TenantDomains.Add(domain);
            await _context.SaveChangesAsync();

            // SQL Server sets CreatedAt using DEFAULT GETDATE().
            return MapToResponse(domain);
        }

        public async Task<List<TenantDomainResponseDto>> GetAllAsync()
        {
            var domains = await GetCurrentTenantDomains()
                .AsNoTracking()
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.DomainName)
                .ThenBy(x => x.TenantDomainId)
                .ToListAsync();

            return domains.Select(MapToResponse).ToList();
        }

        public async Task<TenantDomainResponseDto?> GetByIdAsync(
            int tenantDomainId)
        {
            var domain = await GetCurrentTenantDomains()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantDomainId == tenantDomainId);

            return domain is null ? null : MapToResponse(domain);
        }

        public async Task<TenantDomainResponseDto?> UpdateAsync(
            int tenantDomainId,
            TenantDomainUpdateDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var domains = GetCurrentTenantDomains();
            var userId = GetCurrentUserId();

            // Keep the domain edit and its verification reset together.
            await using var transaction = await _context.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

            var domain = await domains
                .AsTracking()
                .FirstOrDefaultAsync(x => x.TenantDomainId == tenantDomainId);

            if (domain is null)
                return null;

            var domainName = dto.DomainName.Trim().ToLowerInvariant();
            var domainType = dto.DomainType ?? domain.DomainType;

            if (!string.Equals(domain.DomainName, domainName,
                    StringComparison.OrdinalIgnoreCase) ||
                domain.DomainType != domainType)
            {
                domain.IsVerified = false;
                domain.VerificationToken = null;
                domain.VerifiedAt = null;
            }

            domain.DomainName = domainName;
            domain.DomainType = domainType;
            domain.IsPrimary = dto.IsPrimary ?? domain.IsPrimary;
            domain.IsActive = dto.IsActive ?? domain.IsActive;
            domain.UpdatedAt = DateTime.Now;
            domain.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToResponse(domain);
        }

        public async Task<bool> DeleteAsync(int tenantDomainId)
        {
            var domains = GetCurrentTenantDomains();
            var userId = GetCurrentUserId();
            var updatedAt = DateTime.Now;

            var affectedRows = await domains
                .Where(x => x.TenantDomainId == tenantDomainId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.IsActive, false)
                    .SetProperty(x => x.IsPrimary, false)
                    .SetProperty(x => x.VerificationToken, (string?)null)
                    .SetProperty(x => x.UpdatedAt, updatedAt)
                    .SetProperty(x => x.UpdatedBy, userId));

            return affectedRows > 0;
        }

        private IQueryable<TenantDomain> GetCurrentTenantDomains()
        {
            var tenantId = GetCurrentTenantId();

            return _context.TenantDomains
                .Where(x => x.TenantId == tenantId && !x.IsDeleted);
        }

        private ClaimsPrincipal GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Authentication is required.");

            return user;
        }

        private int GetCurrentTenantId()
        {
            var value = GetCurrentUser().FindFirst("TenantId")?.Value;

            if (!int.TryParse(value, out var tenantId) || tenantId <= 0)
                throw new UnauthorizedAccessException(
                    "A valid TenantId claim is required.");

            return tenantId;
        }

        private string GetCurrentUserId()
        {
            var user = GetCurrentUser();
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException(
                    "The authenticated user ID is missing.");

            return userId;
        }

        private static TenantDomainResponseDto MapToResponse(TenantDomain domain)
        {
            return new TenantDomainResponseDto
            {
                TenantDomainId = domain.TenantDomainId,
                TenantId = domain.TenantId,
                DomainName = domain.DomainName,
                DomainType = domain.DomainType,
                IsPrimary = domain.IsPrimary,
                IsVerified = domain.IsVerified,
                VerifiedAt = domain.VerifiedAt,
                IsActive = domain.IsActive,
                IsDeleted = domain.IsDeleted,
                CreatedAt = domain.CreatedAt,
                CreatedBy = domain.CreatedBy,
                UpdatedAt = domain.UpdatedAt,
                UpdatedBy = domain.UpdatedBy
            };
        }
    }
}