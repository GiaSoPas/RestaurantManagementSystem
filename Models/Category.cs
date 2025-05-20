using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
} 