using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.TenantAuditLogs.DTOs;

public class TenantAuditLogCreateDto
{
    [Required]
    [StringLength(100)]
    public string ModuleCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? PluginCode { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [StringLength(150)]
    public string? EntityName { get; set; }

    [StringLength(100)]
    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}