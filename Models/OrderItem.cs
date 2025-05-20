using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int MenuItemId { get; set; }
    
    [Required]
    public int Quantity { get; set; } = 1;
    
    public string? Note { get; set; }
    
    [Required]
    public int StatusId { get; set; } = 1; // Default to "new"
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;
    
    [ForeignKey(nameof(MenuItemId))]
    public virtual MenuItem MenuItem { get; set; } = null!;
    
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
} 