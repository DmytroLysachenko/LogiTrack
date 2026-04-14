using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public string DisplayInfo()
        {
            return $"{Name} | Quantity: {Quantity} | Location: {Location}";
        }
    }
}
