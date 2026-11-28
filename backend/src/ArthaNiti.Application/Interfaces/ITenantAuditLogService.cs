using ArthaNiti.Application.TenantAuditLogs.DTOs;

namespace ArthaNiti.Application.TenantAuditLogs.Interfaces;

public interface ITenantAuditLogService
{
    Task<TenantAuditLogResponseDto> CreateAsync(
        int tenantId,
        string? currentUserId,
        string? ipAddress,
        string? userAgent,
        string? correlationId,
        TenantAuditLogCreateDto dto);

    Task<List<TenantAuditLogResponseDto>> GetAllAsync(
        int tenantId);

    Task<TenantAuditLogResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantAuditLogId);
}