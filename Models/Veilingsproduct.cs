using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Veilingsproduct")]
    public class Veilingsproduct
    {
        [Key]
        public int VeilingsproductID { get; set; }

        [Required]
        [MaxLength(150)]
        public string Naam { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Foto { get; set; }

        [Required]
        public int Hoeveelheid { get; set; }

        public string? Kenmerken { get; set; }

        [MaxLength(100)]
        public string? Aanvoerder { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Prijs { get; set; }

        [Required]
        public int VeilingsID { get; set; }

        // Navigation properties
        [ForeignKey("VeilingsID")]
        public Veiling Veiling { get; set; } = null!;

        public ICollection<Bod> Biedingen { get; set; } = new List<Bod>();
    }
}
