using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(20)]
    public string PaymentMethod { get; set; } = null!; // cash, card, online
    
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;
} 