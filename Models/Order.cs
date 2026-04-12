using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace LogiTrack.Models
{
    public class Order
    {
        static int OrderId;

        static string CustomerName;

        static DateTime DatePlaced;

        static List<InventoryItem> List;

        static void AddItem(InventoryItem item)
        {
            List.Add(item);
        }

        static void RemoveItem(int itemId)
        {
            InventoryItem itemToRemove = List.Find();
            List.Remove(itemToRemove);
        }

        static string GetOrderSummary(int OrderId)
        {
            return $"Order #{OrderId} for {CustomerName} | Items: {List.Count} | Placed: {DatePlaced.ToShortDateString()}";
        }
    }
}
