using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ArthaNiti.Domain.Entites
{
    public class Permission
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string ModuleCode { get; set; }

        public string? PluginCode { get; set; }

        [Required]
        public int? PermissionScope { get; set; }

        [Required]
        public bool? IsSystemPermission { get; set; }

        [Required]
        public bool? IsActive { get; set; }
        [Required]
        public bool? IsDeleted { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

    }
}
