using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class MenuPermissionCreateDto
    {
        [Range(1, int.MaxValue)]
        public required int MenuId { get; set; }

        [Range(1, int.MaxValue)]
        public required int PermissionId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
