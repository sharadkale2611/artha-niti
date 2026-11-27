
using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.TenantUserRoles.DTOs;

public class TenantUserRoleCreateDto
{
    [Range(1, int.MaxValue)]
    public int TenantUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int TenantRoleId { get; set; }
}
