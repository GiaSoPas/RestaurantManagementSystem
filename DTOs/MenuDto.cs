using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
}

public class CreateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
}

public class UpdateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
}

public class MenuItemDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double CookingTime { get; set; }
    
    [Required]
    public bool Available { get; set; }
    
    public string? ImageUrl { get; set; }
}

public class CreateMenuItemDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double CookingTime { get; set; }
    
    [Required]
    public bool Available { get; set; }
    
    public IFormFile? Image { get; set; }
}

public class UpdateMenuItemDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double CookingTime { get; set; }
    
    [Required]
    public bool Available { get; set; }
    
    public IFormFile? Image { get; set; }
}

public class MenuItemImageDto
{
    [Required]
    public IFormFile Image { get; set; } = null!;
}

public class MenuItemsResponseDto
{
    public List<MenuItemDto> Items { get; set; } = new();
}

public class CategoriesResponseDto
{
    public List<CategoryDto> Categories { get; set; } = new();
} 