using System.ComponentModel.DataAnnotations;

namespace Flauction.DTOs
{
    public class CompanyRegisterDTO
    {
        [Required, EmailAddress]
        public string CompanyEmail { get; init; }

        public string Password { get; init; }

        [Required]
        public string CompanyName { get; init; }

        [Required]
        public string Adress { get; init; }

        [Required]
        public string PostalCode { get; init; }

        [Required]
        public string Country { get; init; }
        [Required]
        public string Vat { get; init; }
        [Required]
        public string Iban { get; init; }
        [Required]
        public string BicSwift { get; init; }
    }
}