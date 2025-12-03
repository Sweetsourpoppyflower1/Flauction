using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("ContactPerson")]
    public class ContactPerson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int contactperson_id { get; set; }

        [ForeignKey("company_id")]
        public int company_id { get; set; }

        [Required]
        public string cp_name { get; set; }

        [Required]
        public int cp_phone { get; set; }

        [Required]
        public string cp_email { get; set; }
    }
}
