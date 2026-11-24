using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.Authentication.DTOs
{
    public class TenantUserCreateDto
    {
        [Required]
        public int TenantId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }



        public bool? IsPrimary { get; set; }

        public int? Status { get; set; }

        public DateTime? JoinedAt { get; set; }

        public bool? IsActive { get; set; }
    }
}
