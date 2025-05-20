using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagementSystem.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = null!;
    
    [Required]
    public int RoleId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
} 