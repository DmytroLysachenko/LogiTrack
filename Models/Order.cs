using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        public DateTime DatePlaced { get; set; } = DateTime.UtcNow;
        public List<InventoryItem> Items { get; set; } = new();

        public void AddItem(InventoryItem item)
        {
            Items.Add(item);
        }

        public bool RemoveItem(int inventoryItemId)
        {
            InventoryItem? itemToRemove = Items.FirstOrDefault(item =>
                item.InventoryItemId == inventoryItemId
            );

            if (itemToRemove is null)
            {
                return false;
            }

            Items.Remove(itemToRemove);
            return true;
        }

        public string GetOrderSummary()
        {
            return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced.ToShortDateString()}";
        }
    }
}
