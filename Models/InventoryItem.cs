using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        static int ItemId;
        static string Name;
        static int Quantity;
        static string Location;

        static string DisplayInfo()
        {
            return $"{Name} | Quantity:{Quantity} | Location: {Location}";
        }
    }
}
