using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("AuctionClock")]
    public class AuctionClock
    {
        [Key]
        public int auctionclock_id { get; set; }
        [ForeignKey("auction_id")]
        public int auction_id { get; set; }
        [Required]
        public decimal ac_tick_interval_seconds { get; set; }
        [Required]
        public decimal ac_decrement_amount { get; set; }

        public int ac_final_call_seconds { get; set; } // weet niet of dit moet, hierdoor hebben andere bieders tijd om ook te kopen wanneer iemand voor een prijs kiest
    }
}
