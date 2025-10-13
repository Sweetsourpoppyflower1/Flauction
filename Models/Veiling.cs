using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Veiling")]
    public class Veiling
    {
        [Key]
        public int VeilingsID { get; set; }

        [Required]
        public DateTime StartTijd { get; set; }

        [Required]
        public DateTime EindTijd { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [Required]
        public int VeilingmeesterID { get; set; }

        // Navigation properties
        [ForeignKey("VeilingmeesterID")]
        public Gebruiker Veilingmeester { get; set; } = null!;

        public ICollection<Veilingsproduct> Veilingsproducten { get; set; } = new List<Veilingsproduct>();
        public ICollection<Dashboard> Dashboards { get; set; } = new List<Dashboard>();
    }
}
