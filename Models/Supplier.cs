using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Supplier")]
    public class Supplier : IdentityUser
    {
        //[Key]
        //public int supplier_id { get; set; }
        [Required]
        public string name { get; set; }
        //[Required]
        //public string s_email { get; set; }
        //[Required]
        //public string s_password { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        public string postalcode { get; set; }
        [Required]
        public string country { get; set; }
        [Required]
        public string iban { get; set; }
        [StringLength(500)]
        public string desc { get; set; }

    }
}
