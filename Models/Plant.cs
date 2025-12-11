using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Plant")]
    public class Plant
    {
        [Key]
        public int plant_id { get; set; }
        [ForeignKey("Id")]
        public string supplier_id { get; set; }
        [Required] 
        public string productname { get; set; }
        [Required]
        public string category { get; set; }
        [Required]
        public string form { get; set; }
        [Required]
        public string quality { get; set; }
        [Required]
        public string min_stem { get; set; }
        [Required]
        public string stems_bunch { get; set; }
        [Required]
        public string maturity { get; set; }
        [StringLength(500)]
        public string desc { get; set; }
        [Required]
        public int start_price { get; set; }
        [Required]
        public int min_price { get; set; }

    }
}
