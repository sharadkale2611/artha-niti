
namespace ArthaNiti.Domain.Entities;

public class TenantUserRole
{
    public int TenantUserRoleId { get; set; }

    public int TenantId { get; set; }

    public int TenantUserId { get; set; }

    public int TenantRoleId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
