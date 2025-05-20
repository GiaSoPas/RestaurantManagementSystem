using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TableId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int StatusId { get; set; } = 1; // Default to "new"
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ClosedAt { get; set; }
    
    [ForeignKey(nameof(TableId))]
    public virtual Table Table { get; set; } = null!;
    
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
} 