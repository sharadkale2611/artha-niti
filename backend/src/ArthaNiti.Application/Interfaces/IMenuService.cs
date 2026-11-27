using ArthaNiti.Application.Menus.DTOs;

namespace ArthaNiti.Application.Menus.Interfaces;

public interface IMenuService
{
    Task<MenuResponseDto> CreateAsync(
        string currentUserId,
        MenuCreateDto dto);

    Task<List<MenuResponseDto>> GetAllAsync();

    Task<MenuResponseDto?> GetByIdAsync(int menuId);

    Task<MenuResponseDto?> UpdateAsync(
        int menuId,
        string currentUserId,
        MenuUpdateDto dto);

    Task<bool> DeleteAsync(
        int menuId,
        string currentUserId);
}