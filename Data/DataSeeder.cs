using LogiTrack.Context;
using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Data;

public static class DataSeeder
{
    public static void Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();

        context.Database.Migrate();

        if (context.InventoryItems.Any())
        {
            return;
        }

        context.InventoryItems.Add(
            new InventoryItem
            {
                Name = "Pallet Jack",
                Quantity = 12,
                Location = "Warehouse A",
            }
        );

        context.SaveChanges();
    }
}
