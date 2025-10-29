using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Bod")]
    public class Bod
    {
        [Key]
        public int BodID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Bieder { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Bedrag { get; set; }

        [Required]
        public int VeilingsproductID { get; set; }

        [Required]
        public DateTime Tijdstip { get; set; }

        [Required]
        public int GebruikersID { get; set; }


        [ForeignKey("GebruikersID")]
        public Gebruiker Gebruiker { get; set; } = null!;

        [ForeignKey("VeilingsproductID")]
        public Veilingsproduct Veilingsproduct { get; set; } = null!;
    }
}
