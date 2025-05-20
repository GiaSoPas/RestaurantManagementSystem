using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Data;
using RestaurantManagementSystem.DTOs;

namespace RestaurantManagementSystem.Services;

public class TableService : ITableService
{
    private readonly ApplicationDbContext _context;

    public TableService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TableLayoutDto> GetTableLayoutAsync()
    {
        var tables = await _context.Tables
            .Include(t => t.Status)
            .Include(t => t.CurrentOrder)
                .ThenInclude(o => o!.Status)
            .Include(t => t.CurrentOrder)
                .ThenInclude(o => o!.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
            .OrderBy(t => t.TableNumber)
            .ToListAsync();

        var locations = tables
            .Select(t => t.Location)
            .Where(l => !string.IsNullOrEmpty(l))
            .Distinct()
            .ToList();

        return new TableLayoutDto
        {
            Tables = tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                Status = t.Status.Name,
                StatusColor = t.Status.Color,
                Location = t.Location,
                Description = t.Description,
                CurrentOrder = t.CurrentOrder == null ? null : new OrderResponseDto
                {
                    Id = t.CurrentOrder.Id,
                    TableId = t.CurrentOrder.TableId,
                    TableNumber = t.TableNumber,
                    Status = t.CurrentOrder.Status.Name,
                    CreatedAt = t.CurrentOrder.CreatedAt,
                    ClosedAt = t.CurrentOrder.ClosedAt,
                    TotalPrice = t.CurrentOrder.TotalPrice,
                    Items = t.CurrentOrder.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem.Name,
                        Price = oi.MenuItem.Price,
                        Quantity = oi.Quantity,
                        Status = oi.Status.Name,
                        Note = oi.Note,
                        Subtotal = oi.MenuItem.Price * oi.Quantity
                    }).ToList()
                }
            }).ToList(),
            Locations = locations
        };
    }

    public async Task<TableDto> GetTableAsync(int id)
    {
        var table = await _context.Tables
            .Include(t => t.Status)
            .Include(t => t.CurrentOrder)
                .ThenInclude(o => o!.Status)
            .Include(t => t.CurrentOrder)
                .ThenInclude(o => o!.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (table == null)
            throw new KeyNotFoundException($"Столик с ID {id} не найден");

        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            Capacity = table.Capacity,
            Status = table.Status.Name,
            StatusColor = table.Status.Color,
            Location = table.Location,
            Description = table.Description,
            CurrentOrder = table.CurrentOrder == null ? null : new OrderResponseDto
            {
                Id = table.CurrentOrder.Id,
                TableId = table.CurrentOrder.TableId,
                TableNumber = table.TableNumber,
                Status = table.CurrentOrder.Status.Name,
                CreatedAt = table.CurrentOrder.CreatedAt,
                ClosedAt = table.CurrentOrder.ClosedAt,
                TotalPrice = table.CurrentOrder.TotalPrice,
                Items = table.CurrentOrder.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Price = oi.MenuItem.Price,
                    Quantity = oi.Quantity,
                    Status = oi.Status.Name,
                    Note = oi.Note,
                    Subtotal = oi.MenuItem.Price * oi.Quantity
                }).ToList()
            }
        };
    }

    public async Task<TableHistoryDto> GetTableHistoryAsync(int id, DateTime? startDate = null, DateTime? endDate = null)
    {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == id);

        if (table == null)
            throw new KeyNotFoundException($"Столик с ID {id} не найден");

        var query = _context.Orders
            .Include(o => o.Status)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.TableId == id);

        if (startDate.HasValue)
            query = query.Where(o => o.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.CreatedAt <= endDate.Value);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return new TableHistoryDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            Orders = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                TableId = o.TableId,
                TableNumber = table.TableNumber,
                Status = o.Status.Name,
                CreatedAt = o.CreatedAt,
                ClosedAt = o.ClosedAt,
                TotalPrice = o.TotalPrice,
                Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = oi.MenuItem.Name,
                    Price = oi.MenuItem.Price,
                    Quantity = oi.Quantity,
                    Status = oi.Status.Name,
                    Note = oi.Note,
                    Subtotal = oi.MenuItem.Price * oi.Quantity
                }).ToList()
            }).ToList()
        };
    }

    public async Task<TableDto> UpdateTableStatusAsync(int id, UpdateTableStatusDto dto)
    {
        var table = await _context.Tables
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (table == null)
            throw new KeyNotFoundException($"Столик с ID {id} не найден");

        var status = await _context.TableStatuses.FindAsync(dto.StatusId);
        if (status == null)
            throw new KeyNotFoundException($"Статус с ID {dto.StatusId} не найден");

        table.StatusId = dto.StatusId;
        await _context.SaveChangesAsync();

        return await GetTableAsync(id);
    }

    public async Task<TableDto> MoveOrderAsync(int tableId, int orderId, MoveOrderDto dto)
    {
        var sourceTable = await _context.Tables
            .Include(t => t.CurrentOrder)
            .FirstOrDefaultAsync(t => t.Id == tableId);

        if (sourceTable == null)
            throw new KeyNotFoundException($"Столик с ID {tableId} не найден");

        if (sourceTable.CurrentOrder?.Id != orderId)
            throw new InvalidOperationException($"Заказ с ID {orderId} не найден на столике {tableId}");

        var targetTable = await _context.Tables
            .Include(t => t.CurrentOrder)
            .FirstOrDefaultAsync(t => t.Id == dto.NewTableId);

        if (targetTable == null)
            throw new KeyNotFoundException($"Столик с ID {dto.NewTableId} не найден");

        if (targetTable.CurrentOrder != null)
            throw new InvalidOperationException($"Столик {dto.NewTableId} уже занят");

        var order = sourceTable.CurrentOrder;
        order.TableId = dto.NewTableId;

        sourceTable.CurrentOrderId = null;
        targetTable.CurrentOrderId = orderId;

        // Обновляем статусы столиков
        sourceTable.StatusId = 1; // Свободен
        targetTable.StatusId = 2; // Занят

        await _context.SaveChangesAsync();

        return await GetTableAsync(dto.NewTableId);
    }

    public async Task<List<TableStatusDto>> GetTableStatusesAsync()
    {
        var statuses = await _context.TableStatuses
            .OrderBy(s => s.Id)
            .ToListAsync();

        return statuses.Select(s => new TableStatusDto
        {
            Id = s.Id,
            Name = s.Name,
            Color = s.Color,
            Description = s.Description
        }).ToList();
    }
} 