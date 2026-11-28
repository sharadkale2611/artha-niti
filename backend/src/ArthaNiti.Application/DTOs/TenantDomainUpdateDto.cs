using ArthaNiti.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.DTOs
{
    public class TenantDomainUpdateDto
    {
        [Required, StringLength(253)]
        public required string DomainName { get; set; }
        [EnumDataType(typeof(DomainType))]
        public DomainType? DomainType { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsActive { get; set; }
    }
}
