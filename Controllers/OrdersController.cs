using Microsoft.AspNetCore.Mvc;
using RestaurantManagementSystem.DTOs;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.Controllers;

/// <summary>
/// Контроллер для управления заказами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Создает новый заказ
    /// </summary>
    /// <param name="orderDto">Данные для создания заказа</param>
    /// <returns>Созданный заказ</returns>
    /// <response code="201">Заказ успешно создан</response>
    /// <response code="400">Некорректные данные заказа</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto orderDto)
    {
        try
        {
            // TODO: Получать userId из токена авторизации
            var userId = 1; // Временное решение для тестирования
            var order = await _orderService.CreateOrderAsync(orderDto, userId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Получает информацию о заказе по его ID
    /// </summary>
    /// <param name="id">ID заказа</param>
    /// <returns>Информация о заказе</returns>
    /// <response code="200">Заказ найден</response>
    /// <response code="404">Заказ не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderAsync(id);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Получает список активных заказов
    /// </summary>
    /// <returns>Список активных заказов</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetActiveOrders()
    {
        var orders = await _orderService.GetActiveOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Обновляет статус заказа
    /// </summary>
    /// <param name="id">ID заказа</param>
    /// <param name="statusId">ID нового статуса</param>
    /// <returns>Обновленный заказ</returns>
    /// <response code="200">Статус успешно обновлен</response>
    /// <response code="400">Некорректный статус</response>
    /// <response code="404">Заказ не найден</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(int id, [FromQuery] int statusId)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, statusId);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Обновляет статус блюда в заказе
    /// </summary>
    /// <param name="orderId">ID заказа</param>
    /// <param name="itemId">ID блюда в заказе</param>
    /// <param name="statusId">ID нового статуса</param>
    /// <returns>Обновленный заказ</returns>
    /// <response code="200">Статус успешно обновлен</response>
    /// <response code="400">Некорректный статус</response>
    /// <response code="404">Заказ или блюдо не найдены</response>
    [HttpPut("{orderId}/items/{itemId}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderItemStatus(
        int orderId, int itemId, [FromQuery] int statusId)
    {
        try
        {
            var order = await _orderService.UpdateOrderItemStatusAsync(orderId, itemId, statusId);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Отменяет заказ
    /// </summary>
    /// <param name="id">ID заказа</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Заказ успешно отменен</response>
    /// <response code="400">Невозможно отменить заказ</response>
    /// <response code="404">Заказ не найден</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var success = await _orderService.CancelOrderAsync(id);
            if (!success)
                return BadRequest("Order cannot be cancelled");
                
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 