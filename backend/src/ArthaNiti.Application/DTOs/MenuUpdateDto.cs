using System.ComponentModel.DataAnnotations;
using ArthaNiti.Domain.Enums;

namespace ArthaNiti.Application.Menus.DTOs;

public class MenuUpdateDto
{
    public int? ParentMenuId { get; set; }

    [Required]
    [StringLength(150)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Route { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    [Required]
    [StringLength(100)]
    public string ModuleCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? PluginCode { get; set; }

    public MenuScope MenuScope { get; set; }

    public MenuType MenuType { get; set; }

    public MenuLocation MenuLocation { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; }

    public bool IsActive { get; set; }
}