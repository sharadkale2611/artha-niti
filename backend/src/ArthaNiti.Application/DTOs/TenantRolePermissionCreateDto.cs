
using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.TenantRolePermissions.DTOs;

public class TenantRolePermissionCreateDto
{
    [Range(1, int.MaxValue)]
    public int TenantRoleId { get; set; }

    [Range(1, int.MaxValue)]
    public int PermissionId { get; set; }
}
