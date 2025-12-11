using System.ComponentModel.DataAnnotations;

namespace Flauction.DTOs.Output.RegisterDTOs
{
    public class SupplierRegisterDTO
    {
        [Required, EmailAddress]
        public string SupplierEmail { get; init; }
        [Required]
        public string Password { get; init; }
        [Required]
        public string SupplierName { get; init; }
        [Required]
        public string Address { get; init; }
        [Required]
        public string PostalCode { get; init; }
        [Required]
        public string Country { get; init; }
        [Required]
        public string Iban { get; init; }
        [Required]
        [StringLength(500)]
        public string Desc { get; init; }

    }
}
