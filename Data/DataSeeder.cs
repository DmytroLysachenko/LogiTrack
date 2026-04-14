using LogiTrack.Context;
using LogiTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync("Manager"))
        {
            await roleManager.CreateAsync(new IdentityRole("Manager"));
        }

        if (await context.InventoryItems.AnyAsync())
        {
            return;
        }

        await context.InventoryItems.AddAsync(
            new InventoryItem
            {
                Name = "Pallet Jack",
                Quantity = 12,
                Location = "Warehouse A",
            }
        );

        await context.SaveChangesAsync();
    }
}
