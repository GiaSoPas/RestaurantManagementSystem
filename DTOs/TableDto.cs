using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.DTOs;

public class TableDto
{
    public int Id { get; set; }
    
    [Required]
    public int TableNumber { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int StatusId { get; set; }
    
    [Required]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    public string StatusColor { get; set; } = string.Empty;
    
    [Required]
    public int Capacity { get; set; }
    
    public string? Description { get; set; }
    
    public string? Location { get; set; }
    
    public OrderResponseDto? CurrentOrder { get; set; }
}

public class TableStatusDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateTableStatusDto
{
    [Required]
    public int StatusId { get; set; }
}

public class MoveOrderDto
{
    [Required]
    public int NewTableId { get; set; }
}

public class TableHistoryDto
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public List<OrderResponseDto> Orders { get; set; } = new();
}

public class TableLayoutDto
{
    public List<TableDto> Tables { get; set; } = new();
    public List<string> Locations { get; set; } = new();
} 