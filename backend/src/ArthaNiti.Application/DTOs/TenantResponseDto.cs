using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class TenantResponseDto
    {
        public int TenantId { get; set; }

        public string TenantCode { get; set; }

        public string TenantName { get; set; }

        public string Slug { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}

