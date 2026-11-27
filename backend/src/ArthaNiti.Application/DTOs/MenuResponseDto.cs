using ArthaNiti.Domain.Enums;

namespace ArthaNiti.Application.Menus.DTOs;

public class MenuResponseDto
{
    public int MenuId { get; set; }

    public int? ParentMenuId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Route { get; set; }

    public string? Icon { get; set; }

    public string ModuleCode { get; set; } = string.Empty;

    public string? PluginCode { get; set; }

    public MenuScope MenuScope { get; set; }

    public MenuType MenuType { get; set; }

    public MenuLocation MenuLocation { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; }

    public bool IsSystemMenu { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}