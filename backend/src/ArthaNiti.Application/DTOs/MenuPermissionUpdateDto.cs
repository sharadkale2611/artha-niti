using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class MenuPermissionUpdateDto
    {
        [Range(1, int.MaxValue)]
        public required int MenuId { get; set; }

        [Range(1, int.MaxValue)]
        public required int PermissionId { get; set; }

        public required bool IsActive { get; set; }
    }
}
