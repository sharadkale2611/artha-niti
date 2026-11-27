
namespace ArthaNiti.Application.TenantSettings.DTOs;

public class TenantSettingResponseDto
{
    public int TenantSettingId { get; set; }

    public int TenantId { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string? SettingValue { get; set; }

    public string ValueType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsEncrypted { get; set; }

    public bool IsSystemSetting { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
