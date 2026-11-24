using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace ArthaNiti.Infrastructure.Services
{
    public class TenantUserService : ITenantUserService
    {
        private readonly ArthaNitiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantUserService(
            ArthaNitiDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TenantUserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var entities = await _context.Set<TenantUser>()
                .AsNoTracking()
                .Where(p => p.IsDeleted != true)
                .ToListAsync(cancellationToken);

            return entities.Select(MapToResponse).ToList();
        }

        public async Task<TenantUserResponseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TenantUser>()
                .FirstOrDefaultAsync(
                    p => p.TenantUserId == id && p.IsDeleted != true,
                    cancellationToken);

            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<TenantUserResponseDto> CreateAsync(
            TenantUserCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var entity = new TenantUser();

            CopyMatchingProperties(dto, entity);

            entity.CreatedAt = DateTime.Now;
            entity.CreatedBy = GetCurrentUserId();
            entity.UpdatedAt = null;
            entity.UpdatedBy = null;
            entity.IsDeleted = false;

            _context.Set<TenantUser>().Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return MapToResponse(entity);
        }

        public async Task<bool> UpdateAsync(
            int id,
            TenantUserUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TenantUser>()
                .FirstOrDefaultAsync(
                    p => p.TenantUserId == id && p.IsDeleted != true,
                    cancellationToken);

            if (entity == null)
                return false;

            CopyMatchingProperties(dto, entity);

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TenantUser>()
                .FirstOrDefaultAsync(
                    p => p.TenantUserId == id && p.IsDeleted != true,
                    cancellationToken);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static TenantUserResponseDto MapToResponse(
            TenantUser entity)
        {
            var dto = new TenantUserResponseDto();

            CopyMatchingProperties(entity, dto);

            return dto;
        }

        private static void CopyMatchingProperties(
            object source,
            object destination)
        {
            var sourceProperties = source.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            var destinationProperties = destination.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProperty in sourceProperties)
            {
                if (!sourceProperty.CanRead ||
                    sourceProperty.GetIndexParameters().Length > 0)
                    continue;

                var destinationProperty = destinationProperties.FirstOrDefault(
                    p => p.Name == sourceProperty.Name &&
                         p.CanWrite &&
                         p.GetIndexParameters().Length == 0);

                if (destinationProperty == null)
                    continue;

                var sourceType = sourceProperty.PropertyType;
                var destinationType = destinationProperty.PropertyType;

                var sourceUnderlyingType =
                    Nullable.GetUnderlyingType(sourceType) ?? sourceType;

                var destinationUnderlyingType =
                    Nullable.GetUnderlyingType(destinationType) ?? destinationType;

                if (sourceUnderlyingType != destinationUnderlyingType)
                    continue;

                var value = sourceProperty.GetValue(source);

                if (value == null &&
                    destinationType.IsValueType &&
                    Nullable.GetUnderlyingType(destinationType) == null)
                    continue;

                if (value == null &&
                    destinationProperty.IsDefined(
                        typeof(RequiredAttribute), true))
                    continue;

                destinationProperty.SetValue(destination, value);
            }
        }

        private string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub");
        }
    }
}
