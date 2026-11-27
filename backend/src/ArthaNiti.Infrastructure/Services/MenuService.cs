using ArthaNiti.Application.Menus.DTOs;
using ArthaNiti.Application.Menus.Interfaces;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Domain.Enums;
using ArthaNiti.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArthaNiti.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly ArthaNitiDbContext _context;
    private readonly ILogger<MenuService> _logger;

    public MenuService(
        ArthaNitiDbContext context,
        ILogger<MenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // CREATE
    public async Task<MenuResponseDto> CreateAsync(
        string currentUserId,
        MenuCreateDto dto)
    {
        ValidateEnums(dto.MenuScope, dto.MenuType, dto.MenuLocation);

        await ValidateParentMenuAsync(dto.ParentMenuId);

        string code = dto.Code.Trim();

        bool codeExists = await _context.Menus
            .AnyAsync(x => x.Code == code && !x.IsDeleted);

        if (codeExists)
        {
            _logger.LogWarning("Menu creation failed. Duplicate code: {Code}", code);
            throw new InvalidOperationException("Menu code already exists.");
        }

        var menu = new Menu
        {
            ParentMenuId = dto.ParentMenuId,
            Code = code,
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Route = dto.Route,
            Icon = dto.Icon,
            ModuleCode = dto.ModuleCode.Trim(),
            PluginCode = dto.PluginCode,
            MenuScope = dto.MenuScope,
            MenuType = dto.MenuType,
            MenuLocation = dto.MenuLocation,
            DisplayOrder = dto.DisplayOrder,
            IsVisible = dto.IsVisible,
            IsSystemMenu = dto.IsSystemMenu,
            IsActive = dto.IsActive,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            CreatedBy = currentUserId
        };

        _context.Menus.Add(menu);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Menu created successfully. MenuId: {MenuId}, Code: {Code}",
            menu.MenuId,
            menu.Code);

        return MapToResponse(menu);
    }

    // GET ALL
    public async Task<List<MenuResponseDto>> GetAllAsync()
    {
        var menus = await _context.Menus
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.MenuId)
            .ToListAsync();

        return menus.Select(MapToResponse).ToList();
    }

    // GET BY ID
    public async Task<MenuResponseDto?> GetByIdAsync(int menuId)
    {
        var menu = await _context.Menus
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.MenuId == menuId &&
                !x.IsDeleted);

        return menu == null ? null : MapToResponse(menu);
    }

    // UPDATE
    public async Task<MenuResponseDto?> UpdateAsync(
        int menuId,
        string currentUserId,
        MenuUpdateDto dto)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(x =>
                x.MenuId == menuId &&
                !x.IsDeleted);

        if (menu == null)
            return null;

        if (menu.IsSystemMenu)
        {
            _logger.LogWarning(
                "Update blocked for system menu. MenuId: {MenuId}",
                menuId);

            throw new InvalidOperationException(
                "System menus cannot be updated.");
        }

        ValidateEnums(dto.MenuScope, dto.MenuType, dto.MenuLocation);

        if (dto.ParentMenuId == menuId)
            throw new ArgumentException("Menu cannot be its own parent.");

        await ValidateParentMenuAsync(dto.ParentMenuId);

        string code = dto.Code.Trim();

        bool codeExists = await _context.Menus.AnyAsync(x =>
            x.MenuId != menuId &&
            x.Code == code &&
            !x.IsDeleted);

        if (codeExists)
        {
            _logger.LogWarning(
                "Menu update failed. Duplicate code: {Code}",
                code);

            throw new InvalidOperationException("Menu code already exists.");
        }

        menu.ParentMenuId = dto.ParentMenuId;
        menu.Code = code;
        menu.Name = dto.Name.Trim();
        menu.Description = dto.Description;
        menu.Route = dto.Route;
        menu.Icon = dto.Icon;
        menu.ModuleCode = dto.ModuleCode.Trim();
        menu.PluginCode = dto.PluginCode;
        menu.MenuScope = dto.MenuScope;
        menu.MenuType = dto.MenuType;
        menu.MenuLocation = dto.MenuLocation;
        menu.DisplayOrder = dto.DisplayOrder;
        menu.IsVisible = dto.IsVisible;
        menu.IsActive = dto.IsActive;
        menu.UpdatedAt = DateTime.Now;
        menu.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Menu updated successfully. MenuId: {MenuId}",
            menuId);

        return MapToResponse(menu);
    }

    // DELETE (SOFT DELETE)
    public async Task<bool> DeleteAsync(
        int menuId,
        string currentUserId)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(x =>
                x.MenuId == menuId &&
                !x.IsDeleted);

        if (menu == null)
            return false;

        if (menu.IsSystemMenu)
        {
            _logger.LogWarning(
                "Delete blocked for system menu. MenuId: {MenuId}",
                menuId);

            throw new InvalidOperationException(
                "System menus cannot be deleted.");
        }

        bool hasChildren = await _context.Menus.AnyAsync(x =>
            x.ParentMenuId == menuId &&
            !x.IsDeleted);

        if (hasChildren)
        {
            throw new InvalidOperationException(
                "Cannot delete menu because child menus exist.");
        }

        menu.IsDeleted = true;
        menu.IsActive = false;
        menu.IsVisible = false;
        menu.UpdatedAt = DateTime.Now;
        menu.UpdatedBy = currentUserId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Menu deleted successfully. MenuId: {MenuId}",
            menuId);

        return true;
    }

    // ENUM VALIDATION
    private static void ValidateEnums(
        MenuScope scope,
        MenuType type,
        MenuLocation location)
    {
        if (!Enum.IsDefined(typeof(MenuScope), scope))
            throw new ArgumentException("Invalid MenuScope.");

        if (!Enum.IsDefined(typeof(MenuType), type))
            throw new ArgumentException("Invalid MenuType.");

        if (!Enum.IsDefined(typeof(MenuLocation), location))
            throw new ArgumentException("Invalid MenuLocation.");
    }

    // PARENT MENU VALIDATION
    private async Task ValidateParentMenuAsync(int? parentMenuId)
    {
        if (parentMenuId == null)
            return;

        var parentMenu = await _context.Menus
            .FirstOrDefaultAsync(x =>
                x.MenuId == parentMenuId &&
                !x.IsDeleted &&
                x.IsActive);

        if (parentMenu == null)
            throw new ArgumentException("Parent menu not found or inactive.");

        if (parentMenu.MenuType != MenuType.Group)
            throw new ArgumentException(
                "Parent menu must be a Group type.");
    }

    // ENTITY TO RESPONSE DTO
    private static MenuResponseDto MapToResponse(Menu menu)
    {
        return new MenuResponseDto
        {
            MenuId = menu.MenuId,
            ParentMenuId = menu.ParentMenuId,
            Code = menu.Code,
            Name = menu.Name,
            Description = menu.Description,
            Route = menu.Route,
            Icon = menu.Icon,
            ModuleCode = menu.ModuleCode,
            PluginCode = menu.PluginCode,
            MenuScope = menu.MenuScope,
            MenuType = menu.MenuType,
            MenuLocation = menu.MenuLocation,
            DisplayOrder = menu.DisplayOrder,
            IsVisible = menu.IsVisible,
            IsSystemMenu = menu.IsSystemMenu,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt,
            CreatedBy = menu.CreatedBy,
            UpdatedAt = menu.UpdatedAt,
            UpdatedBy = menu.UpdatedBy
        };
    }
}