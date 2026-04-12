namespace LogiTrack.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public string DisplayInfo()
        {
            return $"{Name} | Quantity: {Quantity} | Location: {Location}";
        }
    }
}
