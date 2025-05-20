using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class Table
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TableNumber { get; set; }
    
    [Required]
    public int Capacity { get; set; }
    
    [Required]
    public int StatusId { get; set; } = 1; // Default to "Available"
    
    public int? CurrentOrderId { get; set; }
    
    public string? Location { get; set; } // Например: "Зал", "Терраса", "Бар"
    
    public string? Description { get; set; }
    
    [ForeignKey(nameof(StatusId))]
    public virtual TableStatus Status { get; set; } = null!;
    
    [ForeignKey(nameof(CurrentOrderId))]
    public virtual Order? CurrentOrder { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class TableStatus
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    
    [Required]
    [StringLength(7)]
    public string Color { get; set; } = "#000000"; // Hex color code
    
    public string? Description { get; set; }
    
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
} 