using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.DTOs;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto orderDto, int userId)
    {
        // Проверяем, свободен ли столик
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == orderDto.TableId && t.IsAvailable);
            
        if (table == null)
            throw new InvalidOperationException("Table is not available");
            
        // Проверяем существование всех блюд
        var menuItemIds = orderDto.Items.Select(i => i.MenuItemId).ToList();
        var menuItems = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.Id) && m.IsAvailable)
            .ToDictionaryAsync(m => m.Id);
            
        if (menuItems.Count != menuItemIds.Count)
            throw new InvalidOperationException("Some menu items are not available");
            
        // Создаем заказ
        var order = new Order
        {
            TableId = orderDto.TableId,
            UserId = userId,
            StatusId = 1, // new
            CreatedAt = DateTime.UtcNow,
            TotalPrice = 0 // будет рассчитано ниже
        };
        
        _context.Orders.Add(order);
        
        // Добавляем элементы заказа
        decimal totalPrice = 0;
        foreach (var item in orderDto.Items)
        {
            var menuItem = menuItems[item.MenuItemId];
            var orderItem = new OrderItem
            {
                Order = order,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                Note = item.Note,
                StatusId = 6, // new (item_status)
                CreatedAt = DateTime.UtcNow
            };
            
            _context.OrderItems.Add(orderItem);
            totalPrice += menuItem.Price * item.Quantity;
        }
        
        order.TotalPrice = totalPrice;
        
        // Помечаем столик как занятый
        table.IsAvailable = false;
        
        await _context.SaveChangesAsync();
        
        return await GetOrderAsync(order.Id);
    }

    public async Task<OrderResponseDto> GetOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Status)
            .FirstOrDefaultAsync(o => o.Id == orderId);
            
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
            
        return MapToOrderResponseDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetActiveOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Status)
            .Where(o => o.StatusId != 4 && o.StatusId != 5) // не completed и не cancelled
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
            
        return orders.Select(MapToOrderResponseDto);
    }

    public async Task<OrderResponseDto> UpdateOrderStatusAsync(int orderId, int statusId)
    {
        var order = await _context.Orders
            .Include(o => o.Status)
            .FirstOrDefaultAsync(o => o.Id == orderId);
            
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
            
        var status = await _context.Statuses
            .FirstOrDefaultAsync(s => s.Id == statusId && s.Type == "order_status");
            
        if (status == null)
            throw new InvalidOperationException("Invalid status ID");
            
        order.StatusId = statusId;
        
        // Если заказ завершен или отменен, освобождаем столик
        if (statusId == 4 || statusId == 5) // completed или cancelled
        {
            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
                table.IsAvailable = true;
                
            order.ClosedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        return await GetOrderAsync(orderId);
    }

    public async Task<OrderResponseDto> UpdateOrderItemStatusAsync(int orderId, int orderItemId, int statusId)
    {
        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Status)
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.Id == orderItemId);
            
        if (orderItem == null)
            throw new KeyNotFoundException($"Order item not found");
            
        var status = await _context.Statuses
            .FirstOrDefaultAsync(s => s.Id == statusId && s.Type == "item_status");
            
        if (status == null)
            throw new InvalidOperationException("Invalid status ID");
            
        orderItem.StatusId = statusId;
        await _context.SaveChangesAsync();
        
        return await GetOrderAsync(orderId);
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.Id == orderId);
            
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
            
        // Проверяем, можно ли отменить заказ
        if (order.StatusId == 4 || order.StatusId == 5) // completed или cancelled
            return false;
            
        order.StatusId = 5; // cancelled
        order.ClosedAt = DateTime.UtcNow;
        
        // Освобождаем столик
        if (order.Table != null)
            order.Table.IsAvailable = true;
            
        await _context.SaveChangesAsync();
        return true;
    }

    private static OrderResponseDto MapToOrderResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            TableId = order.TableId,
            TableNumber = order.Table.TableNumber,
            Status = order.Status.Name,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            ClosedAt = order.ClosedAt,
            Items = order.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                MenuItemId = oi.MenuItemId,
                MenuItemName = oi.MenuItem.Name,
                Price = oi.MenuItem.Price,
                Quantity = oi.Quantity,
                Note = oi.Note,
                Status = oi.Status.Name,
                Subtotal = oi.MenuItem.Price * oi.Quantity
            }).ToList()
        };
    }
} 