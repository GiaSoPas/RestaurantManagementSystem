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

        // Проверяем наличие ролей
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Id = 1, Name = "Администратор" },
                new() { Id = 2, Name = "Официант" },
                new() { Id = 3, Name = "Повар" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Проверяем наличие статусов
        if (!await context.Statuses.AnyAsync())
        {
            var statuses = new List<Status>
            {
                // Order statuses
                new() { Id = 1, Name = "Новый", Type = "order_status" },
                new() { Id = 2, Name = "В обработке", Type = "order_status" },
                new() { Id = 3, Name = "Готовится", Type = "order_status" },
                new() { Id = 4, Name = "Готов", Type = "order_status" },
                new() { Id = 5, Name = "Подано", Type = "order_status" },
                new() { Id = 6, Name = "Отменен", Type = "order_status" },
                
                // Item statuses
                new() { Id = 7, Name = "Новый", Type = "item_status" },
                new() { Id = 8, Name = "Готовится", Type = "item_status" },
                new() { Id = 9, Name = "Готов", Type = "item_status" },
                new() { Id = 10, Name = "Подано", Type = "item_status" },
                new() { Id = 11, Name = "Отменен", Type = "item_status" }
            };
            await context.Statuses.AddRangeAsync(statuses);
            await context.SaveChangesAsync();
        }

        // Проверяем наличие статусов столиков
        if (!await context.TableStatuses.AnyAsync())
        {
            var tableStatuses = new List<TableStatus>
            {
                new() 
                { 
                    Id = 1, 
                    Name = "Свободен", 
                    Color = "#4CAF50", // Зеленый
                    Description = "Столик свободен и готов к обслуживанию"
                },
                new() 
                { 
                    Id = 2, 
                    Name = "Занят", 
                    Color = "#FF9800", // Оранжевый
                    Description = "За столиком есть активный заказ"
                },
                new() 
                { 
                    Id = 3, 
                    Name = "Ожидает оплаты", 
                    Color = "#F44336", // Красный
                    Description = "Заказ завершен, ожидается оплата"
                },
                new() 
                { 
                    Id = 4, 
                    Name = "Забронирован", 
                    Color = "#2196F3", // Синий
                    Description = "Столик забронирован на определенное время"
                },
                new() 
                { 
                    Id = 5, 
                    Name = "Недоступен", 
                    Color = "#9E9E9E", // Серый
                    Description = "Столик временно недоступен"
                }
            };
            await context.TableStatuses.AddRangeAsync(tableStatuses);
            await context.SaveChangesAsync();
        }

        // Проверяем наличие столиков
        if (!await context.Tables.AnyAsync())
        {
            var tables = new List<Table>
            {
                new() { TableNumber = 1, Capacity = 2, StatusId = 1, Location = "Зал" },
                new() { TableNumber = 2, Capacity = 2, StatusId = 1, Location = "Зал" },
                new() { TableNumber = 3, Capacity = 4, StatusId = 1, Location = "Зал" },
                new() { TableNumber = 4, Capacity = 4, StatusId = 1, Location = "Терраса" },
                new() { TableNumber = 5, Capacity = 6, StatusId = 1, Location = "Терраса" }
            };
            await context.Tables.AddRangeAsync(tables);
            await context.SaveChangesAsync();
        }

        // Проверяем наличие категорий
        if (!await context.Categories.AnyAsync())
        {
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
        }

        // Проверяем наличие блюд
        if (!await context.MenuItems.AnyAsync())
        {
            var categories = await context.Categories.ToListAsync();
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
        }

        // Проверяем наличие тестового пользователя
        if (!await context.Users.AnyAsync())
        {
            var user = new User
            {
                Username = "admin",
                PasswordHash = "admin", // В реальном приложении здесь должен быть хеш пароля
                RoleId = 1 // Администратор
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Создаем тестовый заказ
            var tables = await context.Tables.ToListAsync();
            var menuItems = await context.MenuItems.ToListAsync();
            
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
} 