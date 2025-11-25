using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Auction")]
    public class Auction
    {
        [Key]
        public int auction_id { get; set; }
        [ForeignKey("auctionmaster_id")]
        public int auctionmaster_id { get; set; }
        [ForeignKey("plant_id")]
        public int plant_id { get; set; }
        [Required]
        public string au_status { get; set; }
        [Required]
        public DateTime au_start_time { get; set; }
        public DateTime au_end_time { get; set; }
        [Required]
        public int au_start_price { get; set; }
        public decimal au_final_price { get; set; }
    }
}
