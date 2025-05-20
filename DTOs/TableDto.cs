using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.DTOs;

public class TableDto
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = null!;
    public string StatusColor { get; set; } = null!;
    public string? Location { get; set; }
    public string? Description { get; set; }
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