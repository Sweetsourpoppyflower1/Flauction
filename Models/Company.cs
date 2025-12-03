using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Company")]
    public class Company : IdentityUser
    {
        //[Key]
        //public int company_id { get; set; }
        //[Required]
        //public string c_name { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        public string postalcode { get; set; }
        [Required]
        public string country { get; set; }
        [Required]
        public string vat { get; set; }
        [Required]
        public string iban { get; set; }
        [Required]
        public string bicswift { get; set; }
        //[Required]
        //public string c_password { get; set; }

    }
}
