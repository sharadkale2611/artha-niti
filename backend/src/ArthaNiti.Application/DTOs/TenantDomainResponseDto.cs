using ArthaNiti.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class TenantDomainResponseDto
    {
        public int TenantDomainId { get; set; }
        public int TenantId { get; set; }
        public required string DomainName { get; set; }
        public DomainType DomainType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
