
namespace ArthaNiti.Domain.Entities;

public class TenantUser
{
    public int TenantUserId { get; set; }

    public int TenantId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;

    public int Status { get; set; } = 1;

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
