using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Flauction.DTOs
{
    public class CompanyRegisterDTO
    {
        [Required, EmailAddress]
        public string CompanyEmail { get; init; }

        [Required, MinLength(6)]
        public string Password { get; init; }

        [Required]
        public string CompanyName { get; init; }

        [Required]
        public string Adress { get; init; }

        [Required]
        public string PostalCode { get; init; }

        [Required]
        public string Country { get; init; }

        public string Vat { get; init; }
        public string Iban { get; init; }
        public string BicSwift { get; init; }
    }
}