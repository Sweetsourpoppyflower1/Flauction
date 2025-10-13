using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Gebruiker")]
    public class Gebruiker
    {
        [Key]
        public int GebruikersID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Gebruikersnaam { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Wachtwoord { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Rol { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();
        public ICollection<Bod> Biedingen { get; set; } = new List<Bod>();
    }
}
