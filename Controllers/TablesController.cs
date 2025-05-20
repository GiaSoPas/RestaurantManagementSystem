using Microsoft.AspNetCore.Mvc;
using RestaurantManagementSystem.DTOs;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.Controllers;

/// <summary>
/// Контроллер для управления столиками ресторана
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    /// <summary>
    /// Получает схему расположения столиков
    /// </summary>
    /// <returns>Схема расположения столиков с их статусами и текущими заказами</returns>
    /// <response code="200">Возвращает схему расположения столиков</response>
    [HttpGet("layout")]
    [ProducesResponseType(typeof(TableLayoutDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TableLayoutDto>> GetTableLayout()
    {
        var layout = await _tableService.GetTableLayoutAsync();
        return Ok(layout);
    }

    /// <summary>
    /// Получает информацию о конкретном столике
    /// </summary>
    /// <param name="id">ID столика</param>
    /// <returns>Информация о столике</returns>
    /// <response code="200">Возвращает информацию о столике</response>
    /// <response code="404">Столик не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TableDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableDto>> GetTable(int id)
    {
        try
        {
            var table = await _tableService.GetTableAsync(id);
            return Ok(table);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Получает историю заказов для столика
    /// </summary>
    /// <param name="id">ID столика</param>
    /// <param name="startDate">Начальная дата для фильтрации (опционально)</param>
    /// <param name="endDate">Конечная дата для фильтрации (опционально)</param>
    /// <returns>История заказов столика</returns>
    /// <response code="200">Возвращает историю заказов</response>
    /// <response code="404">Столик не найден</response>
    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(TableHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableHistoryDto>> GetTableHistory(
        int id,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var history = await _tableService.GetTableHistoryAsync(id, startDate, endDate);
            return Ok(history);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Обновляет статус столика
    /// </summary>
    /// <param name="id">ID столика</param>
    /// <param name="dto">Данные для обновления статуса</param>
    /// <returns>Обновленная информация о столике</returns>
    /// <response code="200">Статус столика успешно обновлен</response>
    /// <response code="404">Столик или статус не найден</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(TableDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TableDto>> UpdateTableStatus(int id, UpdateTableStatusDto dto)
    {
        try
        {
            var table = await _tableService.UpdateTableStatusAsync(id, dto);
            return Ok(table);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Перемещает заказ с одного столика на другой
    /// </summary>
    /// <param name="tableId">ID исходного столика</param>
    /// <param name="orderId">ID заказа</param>
    /// <param name="dto">Данные для перемещения заказа</param>
    /// <returns>Информация о целевом столике</returns>
    /// <response code="200">Заказ успешно перемещен</response>
    /// <response code="404">Столик не найден</response>
    /// <response code="400">Невозможно переместить заказ</response>
    [HttpPost("{tableId}/orders/{orderId}/move")]
    [ProducesResponseType(typeof(TableDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TableDto>> MoveOrder(int tableId, int orderId, MoveOrderDto dto)
    {
        try
        {
            var table = await _tableService.MoveOrderAsync(tableId, orderId, dto);
            return Ok(table);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Получает список всех возможных статусов столиков
    /// </summary>
    /// <returns>Список статусов столиков</returns>
    /// <response code="200">Возвращает список статусов</response>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(List<TableStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TableStatusDto>>> GetTableStatuses()
    {
        var statuses = await _tableService.GetTableStatusesAsync();
        return Ok(statuses);
    }
} 