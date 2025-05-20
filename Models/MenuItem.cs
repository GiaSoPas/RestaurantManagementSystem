using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class MenuItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    [StringLength(255)]
    public string? ImageUrl { get; set; }
    
    public TimeSpan? PreparationTime { get; set; }
    
    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 