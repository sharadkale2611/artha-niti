using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class TenantUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string TenantCode { get; set; }

        [Required]
        [StringLength(200)]
        public string TenantName { get; set; }

        [Required]
        [StringLength(150)]
        public string Slug { get; set; }

        [StringLength(256)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        public int? Status { get; set; }

        public bool? IsActive { get; set; }
    }
}
