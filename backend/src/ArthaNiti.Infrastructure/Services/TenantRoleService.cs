using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ArthaNiti.Infrastructure.Services
{
    public class TenantRoleService : ITenantRoleService
    {
        private readonly ArthaNitiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantRoleService(
            ArthaNitiDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        public async Task<List<TenantRoleResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await GetTenantRoles()
                .AsNoTracking()
                .OrderBy(x => x.TenantRoleId)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToResponse).ToList();
        }

        // GET BY ID
        public async Task<TenantRoleResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetTenantRoles()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.TenantRoleId == id,
                    cancellationToken);

            return entity == null ? null : MapToResponse(entity);
        }

        // CREATE
        public async Task<TenantRoleResponseDto> CreateAsync(
            TenantRoleCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new TenantRole
            {
                TenantId = GetCurrentTenantId(),
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                IsSystemRole = dto.IsSystemRole,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = GetCurrentUserId()
            };

            _context.Set<TenantRole>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToResponse(entity);
        }

        // UPDATE
        public async Task<bool> UpdateAsync(
    int id,
    TenantRoleUpdateDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var code = dto.Code.Trim();
            var name = dto.Name.Trim();
            var description = dto.Description?.Trim();
            var updatedBy = GetCurrentUserId();
            var updatedAt = DateTime.UtcNow;

            var affectedRows = await GetTenantRoles()
                .Where(x => x.TenantRoleId == id)
                .ExecuteUpdateAsync(setters =>
                {
                    setters.SetProperty(x => x.Code, code);
                    setters.SetProperty(x => x.Name, name);
                    setters.SetProperty(x => x.Description, description);
                    setters.SetProperty(x => x.UpdatedBy, updatedBy);
                    setters.SetProperty(x => x.UpdatedAt, updatedAt);

                    if (dto.IsSystemRole is bool isSystemRole)
                    {
                        setters.SetProperty(x => x.IsSystemRole, isSystemRole);
                    }

                    if (dto.IsActive is bool isActive)
                    {
                        setters.SetProperty(x => x.IsActive, isActive);
                    }
                }, cancellationToken);

            return affectedRows > 0;
        }

        // SOFT DELETE
        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var affectedRows = await GetTenantRoles()
                .Where(x => x.TenantRoleId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.IsActive, false)
                    .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
                    .SetProperty(x => x.UpdatedBy, userId),
                    cancellationToken);

            return affectedRows > 0;
        }

        // UPDATE ACTIVE STATUS
        public async Task<bool> UpdateStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var affectedRows = await GetTenantRoles()
                .Where(x => x.TenantRoleId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsActive, isActive)
                    .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
                    .SetProperty(x => x.UpdatedBy, userId),
                    cancellationToken);

            return affectedRows > 0;
        }

        private IQueryable<TenantRole> GetTenantRoles()
        {
            var tenantId = GetCurrentTenantId();

            return _context.Set<TenantRole>()
                .Where(x => x.TenantId == tenantId && x.IsDeleted == false);
        }

        private int GetCurrentTenantId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var claim = user?.FindFirst("TenantId")?.Value;

            if (user?.Identity?.IsAuthenticated != true ||
                !int.TryParse(claim, out var tenantId) ||
                tenantId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "A valid TenantId claim is required.");
            }

            return tenantId;
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException(
                    "The authenticated user ID is missing.");
        }

        private static TenantRoleResponseDto MapToResponse(TenantRole entity)
        {
            return new TenantRoleResponseDto
            {
                TenantRoleId = entity.TenantRoleId,
                TenantId = entity.TenantId,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                IsSystemRole = entity.IsSystemRole == true,
                IsActive = entity.IsActive == true,
                IsDeleted = entity.IsDeleted == true,
                CreatedAt = entity.CreatedAt
                    ?? throw new InvalidOperationException(
                        "CreatedAt is missing for the tenant role."),
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}