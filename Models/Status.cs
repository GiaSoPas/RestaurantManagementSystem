using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RestaurantManagementSystem.Models;

[Index(nameof(Name), nameof(Type), IsUnique = true)]
public class Status
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    
    [Required]
    [StringLength(20)]
    public string Type { get; set; } = null!; // order_status or item_status
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 