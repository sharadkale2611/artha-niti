using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArthaNiti.Domain.Entities;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TenantId { get; set; }

    public string TenantCode { get; set; }

    public string TenantName { get; set; }

    public string Slug { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    [Required]
    public int? Status { get; set; }

    [Required]
    public bool? IsActive { get; set; }

    [Required]
    public bool? IsDeleted { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}


