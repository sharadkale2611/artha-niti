using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class MenuPermissionResponseDto
    {
        public int MenuPermissionId { get; set; }

        public int MenuId { get; set; }

        public int PermissionId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
