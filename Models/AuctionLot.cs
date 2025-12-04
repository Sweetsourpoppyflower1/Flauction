using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("AuctionLot")]
    public class AuctionLot
    {
        [Key]
        public int auctionlot_id { get; set; }
        [ForeignKey("auction_id")]
        public int auction_id { get; set; }
        [ForeignKey("image_id")]
        public int image_id { get; set; }
        [Required]
        public int unit_per_container { get; set; }
        [Required]
        public int containers_in_lot { get; set; }
        [Required]
        public int min_pickup { get; set; }
        [Required]
        public int fustcode { get; set; }
        [Required]
        public int total_quantity { get; set; }
        [Required]
        public int remaining_quantity { get; set; }
    }
}
