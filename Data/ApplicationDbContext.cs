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
    }
} 