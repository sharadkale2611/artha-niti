using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class PermissionResponseDto
    {
        public int PermissionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string ModuleCode { get; set; }

        public string? PluginCode { get; set; }

        public int PermissionScope { get; set; }

        public bool IsSystemPermission { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
