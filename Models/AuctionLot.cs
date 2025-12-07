using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("AuctionLot")]
    public class AuctionLot
    {
        [Key]
        public int auctionlot_id { get; set; }
        [ForeignKey("plant_id")]
        public int plant_id { get; set; }
        [Required]
        public int unit_per_container { get; set; }
        [Required]
        public int containers_in_lot { get; set; }
        [Required]
        public int min_pickup { get; set; }
        [Required]
        public int start_quantity { get; set; }
        [Required]
        public int remaining_quantity { get; set; }
    }
}
