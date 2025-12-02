using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Supplier")]
    public class Supplier : IdentityUser
    {
        [Key]
        public int supplier_id { get; set; }
        [Required]
        public string s_name { get; set; }
        [Required]
        public string s_email { get; set; }
        [Required]
        public string s_password { get; set; }
        [Required]
        public string s_address { get; set; }
        [Required]
        public string s_postalcode { get; set; }
        [Required]
        public string s_country { get; set; }
        [Required]
        public string s_iban { get; set; }
        [StringLength(500)]
        public string s_desc { get; set; }

    }
}
