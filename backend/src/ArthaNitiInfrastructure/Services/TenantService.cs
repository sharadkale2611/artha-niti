using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace ArthaNiti.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private readonly ArthaNitiDbContext _context;

        public TenantService(ArthaNitiDbContext context)
        {
            _context = context;
        }
        public async Task<TenantResponseDto> CreateAsync(TenantCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new Tenant();

            CopyMatchingProperties(dto, entity);

            _context.Set<Tenant>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Tenant>()
                .FindAsync(new object?[] { id }, cancellationToken);

            if (entity == null)
                return false;

            _context.Set<Tenant>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<TenantResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {

            var entities = await _context.Set<Tenant>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities.Select(MapToResponse).ToList();
        }

        public async Task<TenantResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Tenant>()
                 .FindAsync(new object?[] { id }, cancellationToken);

            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<bool> UpdateAsync(int id, TenantUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<Tenant>()
                .FindAsync(new object?[] { id }, cancellationToken);

            if (entity == null)
                return false;

            CopyMatchingProperties(dto, entity);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static TenantResponseDto MapToResponse(
            Tenant entity)
        {
            var dto = new TenantResponseDto();
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

                // Omitted value for a required column: keep the stored value
                // (update) or let the database default apply (create).
                if (value == null &&
                    destinationProperty.IsDefined(typeof(RequiredAttribute), true))
                    continue;

                destinationProperty.SetValue(destination, value);
            }
        }
    }
}
