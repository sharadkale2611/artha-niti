using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ArthaNiti.Domain.Entities
{
    public class TenantUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TenantUserId { get; set; }

        [DatabaseDefault("1")]
        [Required]
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        [DatabaseDefault("0")]
        [Required]
        public bool? IsPrimary { get; set; }

        [DatabaseDefault("1")]
        [Required]
        public int? Status { get; set; }

        [DatabaseDefault("GETDATE()")]
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime? JoinedAt { get; set; }

        [DatabaseDefault("1")]
        [Required]
        public bool? IsActive { get; set; }

        [DatabaseDefault("0")]
        [Required]
        public bool? IsDeleted { get; set; }

        [DatabaseDefault("GETDATE()")]
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }
    }
}
