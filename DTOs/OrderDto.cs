using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.DTOs;

public class CreateOrderDto
{
    [Required]
    public int TableId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int GuestCount { get; set; }
    
    public string? ClientName { get; set; }
    public string? ClientPhone { get; set; }
    public string? ClientEmail { get; set; }
    
    [Required]
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    [Required]
    public int MenuItemId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public string? Note { get; set; }
}

public class OrderResponseDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int TableNumber { get; set; }
    public int GuestCount { get; set; }
    public string? ClientName { get; set; }
    public string? ClientPhone { get; set; }
    public string? ClientEmail { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = null!;
    public decimal Subtotal { get; set; }
} 