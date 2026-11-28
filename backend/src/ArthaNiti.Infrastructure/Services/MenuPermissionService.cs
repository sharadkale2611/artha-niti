using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ArthaNiti.Infrastructure.Services
{
    public class MenuPermissionService : IMenuPermissionService
    {
        private readonly ArthaNitiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuPermissionService(
            ArthaNitiDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<MenuPermissionResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var assignments = await _context.MenuPermissions
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.MenuId)
                .ThenBy(x => x.PermissionId)
                .ToListAsync(cancellationToken);

            return assignments.Select(MapToResponse).ToList();
        }

        public async Task<MenuPermissionResponseDto?> GetByIdAsync(
            int menuPermissionId,
            CancellationToken cancellationToken = default)
        {
            var assignment = await _context.MenuPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.MenuPermissionId == menuPermissionId && !x.IsDeleted,
                    cancellationToken);

            return assignment is null ? null : MapToResponse(assignment);
        }

        public async Task<MenuPermissionResponseDto> CreateAsync(
            MenuPermissionCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // The SQL unique constraint also covers soft-deleted assignments.
            var assignment = await _context.MenuPermissions
                .SingleOrDefaultAsync(
                    x => x.MenuId == dto.MenuId &&
                         x.PermissionId == dto.PermissionId,
                    cancellationToken);

            if (assignment is null)
            {
                assignment = new MenuPermission
                {
                    MenuId = dto.MenuId,
                    PermissionId = dto.PermissionId,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId()
                };

                _context.MenuPermissions.Add(assignment);
            }
            else
            {
                if (!assignment.IsDeleted)
                    throw new InvalidOperationException(
                        "This permission is already assigned to the menu.");

                assignment.IsDeleted = false;
                assignment.IsActive = dto.IsActive;
                SetUpdateAudit(assignment);
            }

            await SaveAssignmentAsync(cancellationToken);
            return MapToResponse(assignment);
        }

        public async Task<MenuPermissionResponseDto?> UpdateAsync(
            int menuPermissionId,
            MenuPermissionUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var assignment = await FindAssignmentAsync(
                menuPermissionId, cancellationToken);

            if (assignment is null)
                return null;

            assignment.MenuId = dto.MenuId;
            assignment.PermissionId = dto.PermissionId;
            assignment.IsActive = dto.IsActive;
            SetUpdateAudit(assignment);

            await SaveAssignmentAsync(cancellationToken);
            return MapToResponse(assignment);
        }

        public async Task<bool> UpdateStatusAsync(
            int menuPermissionId,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var assignment = await FindAssignmentAsync(
                menuPermissionId, cancellationToken);

            if (assignment is null)
                return false;

            assignment.IsActive = isActive;
            SetUpdateAudit(assignment);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(
            int menuPermissionId,
            CancellationToken cancellationToken = default)
        {
            var assignment = await FindAssignmentAsync(
                menuPermissionId, cancellationToken);

            if (assignment is null)
                return false;

            assignment.IsDeleted = true;
            assignment.IsActive = false;
            SetUpdateAudit(assignment);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private Task<MenuPermission?> FindAssignmentAsync(
            int menuPermissionId,
            CancellationToken cancellationToken)
        {
            return _context.MenuPermissions.FirstOrDefaultAsync(
                x => x.MenuPermissionId == menuPermissionId && !x.IsDeleted,
                cancellationToken);
        }

        private async Task SaveAssignmentAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sql &&
                (sql.Number == 2601 || sql.Number == 2627) &&
                sql.Message.Contains(
                    "UQ_MenuPermissions_Assignment",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "An assignment already exists for this menu and permission.", ex);
            }
        }

        private void SetUpdateAudit(MenuPermission assignment)
        {
            assignment.UpdatedAt = DateTime.UtcNow;
            assignment.UpdatedBy = GetCurrentUserId();
        }

        private string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value;
        }

        private static MenuPermissionResponseDto MapToResponse(
            MenuPermission assignment)
        {
            return new MenuPermissionResponseDto
            {
                MenuPermissionId = assignment.MenuPermissionId,
                MenuId = assignment.MenuId,
                PermissionId = assignment.PermissionId,
                IsActive = assignment.IsActive,
                IsDeleted = assignment.IsDeleted,
                CreatedAt = assignment.CreatedAt,
                CreatedBy = assignment.CreatedBy,
                UpdatedAt = assignment.UpdatedAt,
                UpdatedBy = assignment.UpdatedBy
            };
        }
    }
}
