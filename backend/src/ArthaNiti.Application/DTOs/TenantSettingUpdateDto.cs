
using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.TenantSettings.DTOs;

public class TenantSettingUpdateDto
{
    public string? SettingValue { get; set; }

    [Required]
    [MaxLength(30)]
    public string ValueType { get; set; } = "String";

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsEncrypted { get; set; }

    public bool IsSystemSetting { get; set; }

    public bool IsActive { get; set; }
}
