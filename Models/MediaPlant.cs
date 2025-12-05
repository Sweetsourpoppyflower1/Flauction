using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("MediaPlant")]
    public class MediaPlant
    {
        [Key]
        public int mediaplant_id { get; set; }
        [ForeignKey("plant_id")]
        public int plant_id { get; set; }
        [Required]
        public string url { get; set; }
        [Required]
        public string alt_text { get; set; }
        public Boolean is_primary { get; set; }
    }
}
