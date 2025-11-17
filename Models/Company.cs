    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flauction.Models
{
    [Table("Company")]
    public class Company
    {
        [Key]
        public int company_id { get; set; }
        [Required]
        public string c_name { get; set; }
        [Required]
        public string c_address { get; set; }
        [Required]
        public string c_postalcode { get; set; }
        [Required]
        public string c_country { get; set; }
        [Required]
        public string c_vat { get; set; }
        [Required]
        public string c_iban { get; set; }
        [Required]
        public string c_bicswift { get; set; }
        [Required]
        public string c_password { get; set; }
    }
}
