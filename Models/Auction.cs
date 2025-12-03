using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Auction")]
    public class Auction
    {
        [Key]
        public int auction_id { get; set; }
        [ForeignKey("Id")]
        public string auctionmaster_id { get; set; }
        [ForeignKey("plant_id")]
        public int plant_id { get; set; }
        [Required]
        public string status { get; set; }
        [Required]
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        [Required]
        public int start_price { get; set; }
        public decimal final_price { get; set; }
        [Required]
        public int min_price { get; set; }
    }
}
