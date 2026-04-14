using LogiTrack.Context;
using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private const string InventoryCacheKey = "inventory-list";

    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;

    public InventoryController(LogiTrackContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
    {
        var items = await _cache.GetOrCreateAsync(
            InventoryCacheKey,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);

                return await _context.InventoryItems.AsNoTracking().ToListAsync();
            }
        );

        return items ?? [];
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItem item)
    {
        item.InventoryItemId = 0;
        item.Order = null;

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();
        _cache.Remove(InventoryCacheKey);

        return CreatedAtAction(nameof(GetInventoryItems), new { id = item.InventoryItemId }, item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);

        if (item is null)
        {
            return NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Inventory item not found",
                    Detail = $"No inventory item exists with ID {id}.",
                }
            );
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();
        _cache.Remove(InventoryCacheKey);

        return NoContent();
    }
}
