using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Применяем миграции, если они есть
        await context.Database.MigrateAsync();

        // Проверяем, есть ли уже данные
        if (await context.Tables.AnyAsync())
        {
            return; // База данных уже заполнена
        }

        // Добавляем тестового пользователя
        var user = new User
        {
            Username = "admin",
            PasswordHash = "admin", // В реальном приложении здесь должен быть хеш пароля
            RoleId = 1 // Администратор
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Добавляем категории блюд
        var categories = new List<Category>
        {
            new() { Name = "Закуски", Description = "Холодные и горячие закуски" },
            new() { Name = "Супы", Description = "Первые блюда" },
            new() { Name = "Горячие блюда", Description = "Основные блюда" },
            new() { Name = "Десерты", Description = "Сладкие блюда" },
            new() { Name = "Напитки", Description = "Горячие и холодные напитки" }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Добавляем блюда
        var menuItems = new List<MenuItem>
        {
            new() { 
                Name = "Цезарь с курицей", 
                Description = "Классический салат с куриным филе, листьями салата, сухариками и соусом Цезарь",
                Price = 450,
                CategoryId = categories[0].Id,
                IsAvailable = true,
                PreparationTime = TimeSpan.FromMinutes(15)
            },
            new() { 
                Name = "Борщ", 
                Description = "Традиционный украинский борщ со сметаной",
                Price = 350,
                CategoryId = categories[1].Id,
                IsAvailable = true,
                PreparationTime = TimeSpan.FromMinutes(30)
            },
            new() { 
                Name = "Стейк Рибай", 
                Description = "Сочный стейк из мраморной говядины с овощами гриль",
                Price = 1200,
                CategoryId = categories[2].Id,
                IsAvailable = true,
                PreparationTime = TimeSpan.FromMinutes(25)
            },
            new() { 
                Name = "Тирамису", 
                Description = "Классический итальянский десерт с кофе и маскарпоне",
                Price = 350,
                CategoryId = categories[3].Id,
                IsAvailable = true,
                PreparationTime = TimeSpan.FromMinutes(10)
            },
            new() { 
                Name = "Латте", 
                Description = "Кофейный напиток с молоком",
                Price = 250,
                CategoryId = categories[4].Id,
                IsAvailable = true,
                PreparationTime = TimeSpan.FromMinutes(5)
            }
        };
        await context.MenuItems.AddRangeAsync(menuItems);
        await context.SaveChangesAsync();

        // Добавляем столики
        var tables = new List<Table>
        {
            new() { TableNumber = 1, Capacity = 2, IsAvailable = true },
            new() { TableNumber = 2, Capacity = 2, IsAvailable = true },
            new() { TableNumber = 3, Capacity = 4, IsAvailable = true },
            new() { TableNumber = 4, Capacity = 4, IsAvailable = true },
            new() { TableNumber = 5, Capacity = 6, IsAvailable = true }
        };
        await context.Tables.AddRangeAsync(tables);
        await context.SaveChangesAsync();

        // Создаем тестовый заказ
        var order = new Order
        {
            TableId = tables[0].Id,
            UserId = user.Id,
            StatusId = 1, // Новый
            CreatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>
            {
                new()
                {
                    MenuItemId = menuItems[0].Id, // Цезарь
                    Quantity = 2,
                    StatusId = 7, // Новый (item_status)
                    Note = "Без сухариков"
                },
                new()
                {
                    MenuItemId = menuItems[2].Id, // Стейк
                    Quantity = 1,
                    StatusId = 7, // Новый (item_status)
                    Note = "Medium rare"
                }
            }
        };
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
    }
} 