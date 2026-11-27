
using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.TenantUsers.DTOs;

public class TenantUserCreateDto
{
    [Required]
    [StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;

    public int Status { get; set; } = 1;
}
