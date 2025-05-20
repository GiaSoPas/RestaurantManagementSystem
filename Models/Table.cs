using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models;

public class Table
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TableNumber { get; set; }
    
    [Required]
    public int Capacity { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
} 