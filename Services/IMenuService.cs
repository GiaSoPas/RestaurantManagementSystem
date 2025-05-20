using Microsoft.AspNetCore.Http;
using RestaurantManagementSystem.DTOs;

namespace RestaurantManagementSystem.Services;

public interface IMenuService
{
    // Categories
    Task<CategoriesResponseDto> GetCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task DeleteCategoryAsync(int id);
    
    // Menu Items
    Task<MenuItemsResponseDto> GetMenuItemsAsync(int[]? categoryIds = null, bool[]? status = null, string? search = null);
    Task<MenuItemDto> GetMenuItemAsync(int id);
    Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto);
    Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
    Task DeleteMenuItemAsync(int id);
    Task<MenuItemDto> UpdateMenuItemImageAsync(int id, IFormFile image);
} 