using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Acceptance")]
    public class Acceptance
    {
        [Key]
        public int acceptance_id { get; set; }
        [ForeignKey("auction_id")]
        public int auction_id { get; set; }
        [ForeignKey("company_id")]
        public int company_id { get; set; }
        [ForeignKey("auction_lot_id")]
        public int auction_lot_id { get; set; }
        [Required]
        public int acc_tick_number { get; set; } //de tick wanneer een company op de knop buy heeft gedrukt
        [Required]
        public decimal acc_accepted_price { get; set; }
        [Required]
        public int acc_accepted_quantity { get; set; }
        [Required]
        public DateTime acc_time { get; set; }
    }
}
