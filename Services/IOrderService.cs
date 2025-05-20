using RestaurantManagementSystem.DTOs;

namespace RestaurantManagementSystem.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto orderDto, int userId);
    Task<OrderResponseDto> GetOrderAsync(int orderId);
    Task<IEnumerable<OrderResponseDto>> GetActiveOrdersAsync();
    Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, int statusId);
    Task<OrderResponseDto> UpdateOrderItemStatusAsync(int orderId, int orderItemId, int statusId);
    Task<bool> CancelOrderAsync(int orderId);
} 