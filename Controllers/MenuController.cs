using Microsoft.AspNetCore.Mvc;
using RestaurantManagementSystem.DTOs;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Получить список всех категорий
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(CategoriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriesResponseDto>> GetCategories()
    {
        var categories = await _menuService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Создать новую категорию
    /// </summary>
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        var category = await _menuService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }

    /// <summary>
    /// Обновить категорию
    /// </summary>
    [HttpPut("categories/{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _menuService.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить категорию
    /// </summary>
    [HttpDelete("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            await _menuService.DeleteCategoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Получить список блюд с возможностью фильтрации
    /// </summary>
    [HttpGet("items")]
    [ProducesResponseType(typeof(MenuItemsResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MenuItemsResponseDto>> GetMenuItems(
        [FromQuery] int[]? categoryIds = null,
        [FromQuery] bool[]? status = null,
        [FromQuery] string? search = null)
    {
        var items = await _menuService.GetMenuItemsAsync(categoryIds, status, search);
        return Ok(items);
    }

    /// <summary>
    /// Получить информацию о конкретном блюде
    /// </summary>
    [HttpGet("items/{id}")]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
    {
        try
        {
            var item = await _menuService.GetMenuItemAsync(id);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Создать новое блюдо
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromForm] CreateMenuItemDto dto)
    {
        try
        {
            var item = await _menuService.CreateMenuItemAsync(dto);
            return CreatedAtAction(nameof(GetMenuItem), new { id = item.Id }, item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Указанная категория не найдена" });
        }
    }

    /// <summary>
    /// Обновить информацию о блюде
    /// </summary>
    [HttpPut("items/{id}")]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(int id, [FromForm] UpdateMenuItemDto dto)
    {
        try
        {
            var item = await _menuService.UpdateMenuItemAsync(id, dto);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить блюдо
    /// </summary>
    [HttpDelete("items/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        try
        {
            await _menuService.DeleteMenuItemAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Обновить изображение блюда
    /// </summary>
    [HttpPut("items/{id}/image")]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuItemDto>> UpdateMenuItemImage(int id, [FromForm] MenuItemImageDto dto)
    {
        try
        {
            var item = await _menuService.UpdateMenuItemImageAsync(id, dto.Image);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 