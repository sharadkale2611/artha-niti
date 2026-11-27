using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArthaNiti.Domain.Entities;

[Table("TenantRoles")]
public class TenantRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TenantRoleId { get; set; }

    public int TenantId { get; set; }

    [Required, StringLength(100)]
    [Column(TypeName = "varchar(100)")]
    public required string Code { get; set; }

    [Required, StringLength(150)]
    [Column(TypeName = "varchar(150)")]
    public required string Name { get; set; }

    [StringLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? Description { get; set; }

    [Required]
    public bool? IsSystemRole { get; set; } = false;

    [Required]
    public bool? IsActive { get; set; } = true;

    [Required]
    public bool? IsDeleted { get; set; } = false;

    [Required]
    [Column(TypeName = "datetime")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }

    [StringLength(450)]
    [Column(TypeName = "varchar(450)")]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(450)]
    [Column(TypeName = "varchar(450)")]
    public string? UpdatedBy { get; set; }
}