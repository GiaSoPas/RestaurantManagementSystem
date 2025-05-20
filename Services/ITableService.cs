using RestaurantManagementSystem.DTOs;

namespace RestaurantManagementSystem.Services;

public interface ITableService
{
    Task<TableLayoutDto> GetTableLayoutAsync();
    Task<TableDto> GetTableAsync(int id);
    Task<TableHistoryDto> GetTableHistoryAsync(int id, DateTime? startDate = null, DateTime? endDate = null);
    Task<TableDto> UpdateTableStatusAsync(int id, UpdateTableStatusDto dto);
    Task<TableDto> MoveOrderAsync(int tableId, int orderId, MoveOrderDto dto);
    Task<List<TableStatusDto>> GetTableStatusesAsync();
} 