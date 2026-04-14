using LogiTrack.Context;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;

    public InventoryController(LogiTrackContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
    {
        return await _context.InventoryItems.AsNoTracking().ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItem item)
    {
        item.InventoryItemId = 0;
        item.Order = null;

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInventoryItems), new { id = item.InventoryItemId }, item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);

        if (item is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Inventory item not found",
                Detail = $"No inventory item exists with ID {id}."
            });
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
