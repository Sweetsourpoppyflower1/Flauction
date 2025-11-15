using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("AuctionMaster")]
    public class AuctionMaster
    {
        [Key]
        public int auctionmaster_id { get; set; }
        [Required]
        public string am_name { get; set; }
        [Required]
        public int am_phone { get; set; }
        [Required]
        public string am_email { get; set; }
        public string am_address { get; set; }
        [Required]
        public string am_password { get; set; }
    }
}
