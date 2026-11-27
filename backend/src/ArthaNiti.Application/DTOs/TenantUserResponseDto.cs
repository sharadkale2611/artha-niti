
namespace ArthaNiti.Application.TenantUsers.DTOs;

public class TenantUserResponseDto
{
    public int TenantUserId { get; set; }

    public int TenantId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int Status { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
