using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Dashboard")]
    public class Dashboard
    {
        [Key]
        public int DashboardID { get; set; }

        [Required]
        public int VeilingsID { get; set; }

        // Navigation properties
        [ForeignKey("VeilingsID")]
        public Veiling Veiling { get; set; } = null!;
    }
}
