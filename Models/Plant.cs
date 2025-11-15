using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Plant")]
    public class Plant
    {
        [Key]
        public int plant_id { get; set; }
        [ForeignKey("supplier_id")]
        public int supplier_id { get; set; }
        [Required] 
        public string p_productname { get; set; }
        [Required]
        public string p_category { get; set; }
        [Required]
        public string p_form { get; set; }
        [Required]
        public string p_quality { get; set; }
        [Required]
        public string p_min_stem { get; set; }
        [Required]
        public string p_stems_bunch { get; set; }
        [Required]
        public string p_maturity { get; set; }
        [StringLength(500)]
        public string p_desc { get; set; }

    }
}
