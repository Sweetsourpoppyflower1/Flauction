using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("PlantPriceHistory")]
    public class PlantPriceHistory
    {
        [Key]
        public int pricehistory_id { get; set; }

        [ForeignKey("plant_id")]
        public int plant_id { get; set; }

        public decimal old_min_price { get; set; }
        public decimal old_start_price { get; set; }

        public decimal new_min_price { get; set; }
        public decimal new_start_price { get; set; }

        public DateTime changed_at { get; set; }

        public string changed_by { get; set; } // Supplier ID

    }
}
