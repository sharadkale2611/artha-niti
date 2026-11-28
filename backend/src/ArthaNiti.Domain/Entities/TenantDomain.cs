using ArthaNiti.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArthaNiti.Domain.Entities;

[Table("TenantDomains", Schema = "dbo")]
public class TenantDomain
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TenantDomainId { get; set; }

    public int TenantId { get; set; }

    [Required, StringLength(253)]
    [Column(TypeName = "varchar(253)")]
    public required string DomainName { get; set; }
    [EnumDataType(typeof(DomainType))]
    public DomainType DomainType { get; set; } = DomainType.Subdomain;

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    [Column(TypeName = "varchar(256)")]
    public string? VerificationToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? VerifiedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "varchar(450)")]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "varchar(450)")]
    public string? UpdatedBy { get; set; }
}