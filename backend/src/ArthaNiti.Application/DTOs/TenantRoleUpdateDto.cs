using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.DTOs
{
    public class TenantRoleUpdateDto
    {
        [Required, StringLength(100)]
        public required string Code { get; set; }

        [Required, StringLength(150)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool? IsSystemRole { get; set; }

        public bool? IsActive { get; set; }
    }
}