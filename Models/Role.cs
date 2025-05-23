using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
} 