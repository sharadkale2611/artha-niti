using System.Security.Claims;
using ArthaNiti.Application.Menus.DTOs;
using ArthaNiti.Application.Menus.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] MenuCreateDto dto)
    {
        string currentUserId = GetCurrentUserId();

        var result = await _menuService.CreateAsync(
            currentUserId,
            dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.MenuId },
            result);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _menuService.GetAllAsync();

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _menuService.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Menu not found." });

        return Ok(result);
    }

    // UPDATE
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] MenuUpdateDto dto)
    {
        string currentUserId = GetCurrentUserId();

        var result = await _menuService.UpdateAsync(
            id,
            currentUserId,
            dto);

        if (result == null)
            return NotFound(new { message = "Menu not found." });

        return Ok(result);
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        string currentUserId = GetCurrentUserId();

        bool result = await _menuService.DeleteAsync(
            id,
            currentUserId);

        if (!result)
            return NotFound(new { message = "Menu not found." });

        return Ok(new { message = "Menu deleted successfully." });
    }

    // CURRENT USER ID
    private string GetCurrentUserId()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException(
                "User ID not found in token.");

        return userId;
    }
}