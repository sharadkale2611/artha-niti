using ArthaNiti.Domain.Enums;

namespace ArthaNiti.Domain.Entities;

public class Menu
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

    public MenuScope MenuScope { get; set; } = MenuScope.Tenant;

    public MenuType MenuType { get; set; } = MenuType.InternalLink;

    public MenuLocation MenuLocation { get; set; } = MenuLocation.Sidebar;

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool IsSystemMenu { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}