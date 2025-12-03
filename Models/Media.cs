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
        public string m_url { get; set; }
        [Required]
        public string m_alt_text { get; set; }
        public Boolean m_is_primary { get; set; }
    }
}
