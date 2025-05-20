using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.DTOs;
using RestaurantManagementSystem.Models;
using System;

namespace RestaurantManagementSystem.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private const string ImagesFolder = "images/menu";

    public MenuService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // Categories
    public async Task<CategoriesResponseDto> GetCategoriesAsync()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();

        return new CategoriesResponseDto { Categories = categories };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Категория с ID {id} не найдена");

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Категория с ID {id} не найдена");

        // Проверяем, есть ли блюда в этой категории
        if (await _context.MenuItems.AnyAsync(m => m.CategoryId == id))
            throw new InvalidOperationException("Нельзя удалить категорию, содержащую блюда");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    // Menu Items
    public async Task<MenuItemsResponseDto> GetMenuItemsAsync(int[]? categoryIds = null, bool[]? status = null, string? search = null)
    {
        var query = _context.MenuItems
            .Include(m => m.Category)
            .AsQueryable();

        if (categoryIds != null && categoryIds.Length > 0)
            query = query.Where(m => categoryIds.Contains(m.CategoryId));

        if (status != null && status.Length > 0)
            query = query.Where(m => status.Contains(m.IsAvailable));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => 
                m.Name.Contains(search) || 
                m.Description != null && m.Description.Contains(search));

        var items = await query
            .OrderBy(m => m.Category.Name)
            .ThenBy(m => m.Name)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                CategoryId = m.CategoryId,
                Price = m.Price,
                CookingTime = m.PreparationTime == null ? 0 : m.PreparationTime.Value.TotalMinutes,
                Available = m.IsAvailable,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync();

        return new MenuItemsResponseDto { Items = items };
    }

    public async Task<MenuItemDto> GetMenuItemAsync(int id)
    {
        var menuItem = await _context.MenuItems
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menuItem == null)
            throw new KeyNotFoundException($"Блюдо с ID {id} не найдено");

        return new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description,
            CategoryId = menuItem.CategoryId,
            Price = menuItem.Price,
            CookingTime = menuItem.PreparationTime?.TotalMinutes ?? 0,
            Available = menuItem.IsAvailable,
            ImageUrl = menuItem.ImageUrl
        };
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        // Проверяем существование категории
        if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            throw new KeyNotFoundException($"Категория с ID {dto.CategoryId} не найдена");

        var menuItem = new MenuItem
        {
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            PreparationTime = TimeSpan.FromMinutes(dto.CookingTime),
            IsAvailable = dto.Available
        };

        if (dto.Image != null)
        {
            menuItem.ImageUrl = await SaveImageAsync(dto.Image);
        }

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        return await GetMenuItemAsync(menuItem.Id);
    }

    public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
            throw new KeyNotFoundException($"Блюдо с ID {id} не найдено");

        // Проверяем существование категории
        if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
            throw new KeyNotFoundException($"Категория с ID {dto.CategoryId} не найдена");

        menuItem.Name = dto.Name;
        menuItem.Description = dto.Description;
        menuItem.CategoryId = dto.CategoryId;
        menuItem.Price = dto.Price;
        menuItem.PreparationTime = TimeSpan.FromMinutes(dto.CookingTime);
        menuItem.IsAvailable = dto.Available;

        if (dto.Image != null)
        {
            // Удаляем старое изображение, если оно есть
            if (!string.IsNullOrEmpty(menuItem.ImageUrl))
            {
                DeleteImage(menuItem.ImageUrl);
            }
            menuItem.ImageUrl = await SaveImageAsync(dto.Image);
        }

        await _context.SaveChangesAsync();

        return await GetMenuItemAsync(id);
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
            throw new KeyNotFoundException($"Блюдо с ID {id} не найдено");

        // Удаляем изображение, если оно есть
        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
        {
            DeleteImage(menuItem.ImageUrl);
        }

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
    }

    public async Task<MenuItemDto> UpdateMenuItemImageAsync(int id, IFormFile image)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
            throw new KeyNotFoundException($"Блюдо с ID {id} не найдено");

        // Удаляем старое изображение, если оно есть
        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
        {
            DeleteImage(menuItem.ImageUrl);
        }

        menuItem.ImageUrl = await SaveImageAsync(image);
        await _context.SaveChangesAsync();

        return await GetMenuItemAsync(id);
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        // Создаем уникальное имя файла
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(_environment.WebRootPath, ImagesFolder, fileName);

        // Создаем директорию, если её нет
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        // Сохраняем файл
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        // Возвращаем относительный путь к файлу
        return $"/{ImagesFolder}/{fileName}";
    }

    private void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
} 