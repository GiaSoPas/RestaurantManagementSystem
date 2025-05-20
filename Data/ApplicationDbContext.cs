using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<TableStatus> TableStatuses { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Status> Statuses { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure unique constraints
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();
            
        modelBuilder.Entity<Table>()
            .HasIndex(t => t.TableNumber)
            .IsUnique();
            
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
            
        modelBuilder.Entity<TableStatus>()
            .HasIndex(s => s.Name)
            .IsUnique();

        // Настраиваем отношения между Order и Table
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Table>()
            .HasOne(t => t.CurrentOrder)
            .WithOne()
            .HasForeignKey<Table>(t => t.CurrentOrderId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // Seed initial data for roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Администратор" },
            new Role { Id = 2, Name = "Официант" },
            new Role { Id = 3, Name = "Повар" }
        );
        
        // Seed initial data for statuses
        modelBuilder.Entity<Status>().HasData(
            // Order statuses
            new Status { Id = 1, Name = "Новый", Type = "order_status" },
            new Status { Id = 2, Name = "В обработке", Type = "order_status" },
            new Status { Id = 3, Name = "Готовится", Type = "order_status" },
            new Status { Id = 4, Name = "Готов", Type = "order_status" },
            new Status { Id = 5, Name = "Подано", Type = "order_status" },
            new Status { Id = 6, Name = "Отменен", Type = "order_status" },
            
            // Item statuses
            new Status { Id = 7, Name = "Новый", Type = "item_status" },
            new Status { Id = 8, Name = "Готовится", Type = "item_status" },
            new Status { Id = 9, Name = "Готов", Type = "item_status" },
            new Status { Id = 10, Name = "Подано", Type = "item_status" },
            new Status { Id = 11, Name = "Отменен", Type = "item_status" }
        );

        // Seed initial data for table statuses
        modelBuilder.Entity<TableStatus>().HasData(
            new TableStatus 
            { 
                Id = 1, 
                Name = "Свободен", 
                Color = "#4CAF50", // Зеленый
                Description = "Столик свободен и готов к обслуживанию"
            },
            new TableStatus 
            { 
                Id = 2, 
                Name = "Занят", 
                Color = "#FF9800", // Оранжевый
                Description = "За столиком есть активный заказ"
            },
            new TableStatus 
            { 
                Id = 3, 
                Name = "Ожидает оплаты", 
                Color = "#F44336", // Красный
                Description = "Заказ завершен, ожидается оплата"
            },
            new TableStatus 
            { 
                Id = 4, 
                Name = "Забронирован", 
                Color = "#2196F3", // Синий
                Description = "Столик забронирован на определенное время"
            },
            new TableStatus 
            { 
                Id = 5, 
                Name = "Недоступен", 
                Color = "#9E9E9E", // Серый
                Description = "Столик временно недоступен"
            }
        );

        // Добавляем столики
        modelBuilder.Entity<Table>().HasData(
            new Table { Id = 1, TableNumber = 1, Capacity = 2, StatusId = 1, Location = "Зал" },
            new Table { Id = 2, TableNumber = 2, Capacity = 2, StatusId = 1, Location = "Зал" },
            new Table { Id = 3, TableNumber = 3, Capacity = 4, StatusId = 1, Location = "Зал" },
            new Table { Id = 4, TableNumber = 4, Capacity = 4, StatusId = 1, Location = "Терраса" },
            new Table { Id = 5, TableNumber = 5, Capacity = 6, StatusId = 1, Location = "Терраса" }
        );
    }
} 