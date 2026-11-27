
namespace ArthaNiti.Application.TenantRolePermissions.DTOs;

public class TenantRolePermissionResponseDto
{
    public int TenantRolePermissionId { get; set; }

    public int TenantId { get; set; }

    public int TenantRoleId { get; set; }

    public int PermissionId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
