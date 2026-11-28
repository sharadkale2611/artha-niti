using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class PermissionUpdateDto
    {
        [Required]
        [StringLength(150)]
        public string Code { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string ModuleCode { get; set; }

        [StringLength(100)]
        public string? PluginCode { get; set; }

        public int? PermissionScope { get; set; }

        public bool? IsSystemPermission { get; set; }

        public bool? IsActive { get; set; }
    }
}
