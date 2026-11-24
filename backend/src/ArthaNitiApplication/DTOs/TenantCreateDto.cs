using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class TenantCreateDto
    {
        [Required, StringLength(50)]
        public required string TenantCode { get; set; }

        [Required, StringLength(200)]
        public required string TenantName { get; set; }

        [Required, StringLength(150)]
        public required string Slug { get; set; }

        [EmailAddress, StringLength(256)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Range(1, 4)]
        public int Status { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}
