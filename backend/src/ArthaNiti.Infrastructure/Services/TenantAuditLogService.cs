using ArthaNiti.Application.TenantAuditLogs.DTOs;
using ArthaNiti.Application.TenantAuditLogs.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArthaNiti.Infrastructure.Services;

public class TenantAuditLogService : ITenantAuditLogService
{
    private readonly ArthaNitiDbContext _context;
    private readonly ILogger<TenantAuditLogService> _logger;

    public TenantAuditLogService(
        ArthaNitiDbContext context,
        ILogger<TenantAuditLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // CREATE AUDIT LOG
    public async Task<TenantAuditLogResponseDto> CreateAsync(
        int tenantId,
        string? currentUserId,
        string? ipAddress,
        string? userAgent,
        string? correlationId,
        TenantAuditLogCreateDto dto)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Invalid TenantId.");

        if (string.IsNullOrWhiteSpace(dto.ModuleCode))
            throw new ArgumentException("ModuleCode is required.");

        if (string.IsNullOrWhiteSpace(dto.Action))
            throw new ArgumentException("Action is required.");

        var auditLog = new TenantAuditLog
        {
            TenantId = tenantId,
            UserId = currentUserId,
            ModuleCode = dto.ModuleCode.Trim(),
            PluginCode = dto.PluginCode,
            Action = dto.Action.Trim(),
            EntityName = dto.EntityName,
            EntityId = dto.EntityId,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
            Description = dto.Description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CorrelationId = correlationId,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.TenantAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Audit log created. AuditLogId: {AuditLogId}, TenantId: {TenantId}, Action: {Action}",
            auditLog.TenantAuditLogId,
            tenantId,
            auditLog.Action);

        return MapToResponse(auditLog);
    }

    // GET ALL AUDIT LOGS
    public async Task<List<TenantAuditLogResponseDto>> GetAllAsync(
        int tenantId)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Invalid TenantId.");

        var auditLogs = await _context.TenantAuditLogs
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.TenantAuditLogId)
            .ToListAsync();

        _logger.LogInformation(
            "Retrieved {Count} audit logs for TenantId: {TenantId}",
            auditLogs.Count,
            tenantId);

        return auditLogs.Select(MapToResponse).ToList();
    }

    // GET AUDIT LOG BY ID
    public async Task<TenantAuditLogResponseDto?> GetByIdAsync(
        int tenantId,
        int tenantAuditLogId)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Invalid TenantId.");

        var auditLog = await _context.TenantAuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.TenantAuditLogId == tenantAuditLogId);

        if (auditLog == null)
            return null;

        return MapToResponse(auditLog);
    }

    // ENTITY TO RESPONSE DTO
    private static TenantAuditLogResponseDto MapToResponse(
        TenantAuditLog auditLog)
    {
        return new TenantAuditLogResponseDto
        {
            TenantAuditLogId = auditLog.TenantAuditLogId,
            TenantId = auditLog.TenantId,
            UserId = auditLog.UserId,
            ModuleCode = auditLog.ModuleCode,
            PluginCode = auditLog.PluginCode,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            OldValues = auditLog.OldValues,
            NewValues = auditLog.NewValues,
            Description = auditLog.Description,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            CorrelationId = auditLog.CorrelationId,
            CreatedAt = auditLog.CreatedAt,
            CreatedBy = auditLog.CreatedBy
        };
    }
}