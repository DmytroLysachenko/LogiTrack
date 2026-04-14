using LogiTrack.Context;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public OrderController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetOrders()
    {
        var orders = await _context
            .Orders.AsNoTracking()
            .Select(order => new OrderSummaryResponse(
                order.OrderId,
                order.CustomerName,
                order.DatePlaced,
                order.Items.Count
            ))
            .ToListAsync();

        return orders;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDetailsResponse>> GetOrderById(int id)
    {
        var order = await _context
            .Orders.AsNoTracking()
            .Where(order => order.OrderId == id)
            .Select(order => new OrderDetailsResponse(
                order.OrderId,
                order.CustomerName,
                order.DatePlaced,
                order
                    .Items.Select(item => new InventoryItemResponse(
                        item.InventoryItemId,
                        item.Name,
                        item.Quantity,
                        item.Location
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Order not found",
                    Detail = $"No order exists with ID {id}.",
                }
            );
        }

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDetailsResponse>> CreateOrder(Order order)
    {
        order.OrderId = 0;

        if (order.DatePlaced == default)
        {
            order.DatePlaced = DateTime.UtcNow;
        }

        foreach (var item in order.Items)
        {
            item.InventoryItemId = 0;
            item.OrderId = null;
            item.Order = null;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var response = ToOrderDetailsResponse(order);

        return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context
            .Orders.Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.OrderId == id);

        if (order is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Order not found",
                    Detail = $"No order exists with ID {id}.",
                }
            );
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static OrderDetailsResponse ToOrderDetailsResponse(Order order)
    {
        return new OrderDetailsResponse(
            order.OrderId,
            order.CustomerName,
            order.DatePlaced,
            order
                .Items.Select(item => new InventoryItemResponse(
                    item.InventoryItemId,
                    item.Name,
                    item.Quantity,
                    item.Location
                ))
                .ToList()
        );
    }
}

public record OrderSummaryResponse(
    int OrderId,
    string CustomerName,
    DateTime DatePlaced,
    int ItemCount
);

public record OrderDetailsResponse(
    int OrderId,
    string CustomerName,
    DateTime DatePlaced,
    List<InventoryItemResponse> Items
);

public record InventoryItemResponse(
    int InventoryItemId,
    string Name,
    int Quantity,
    string Location
);
