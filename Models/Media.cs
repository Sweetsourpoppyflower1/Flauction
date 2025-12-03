using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Media")]
    public class Media
    {
        [Key]
        public int media_id { get; set; }
        [ForeignKey("plant_id")]
        public int plant_id { get; set; }
        [Required]
        public string url { get; set; }
        [Required]
        public string alt_text { get; set; }
        public Boolean is_primary { get; set; }
    }
}
