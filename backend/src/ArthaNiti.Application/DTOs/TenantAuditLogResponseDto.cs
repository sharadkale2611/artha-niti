namespace ArthaNiti.Application.TenantAuditLogs.DTOs;

public class TenantAuditLogResponseDto
{
    public int TenantAuditLogId { get; set; }

    public int TenantId { get; set; }

    public string? UserId { get; set; }

    public string ModuleCode { get; set; } = string.Empty;

    public string? PluginCode { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? EntityName { get; set; }

    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}