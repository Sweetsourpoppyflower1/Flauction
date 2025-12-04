using System.ComponentModel.DataAnnotations;

namespace Flauction.DTOs.Input
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; init; }

        [Required, MinLength(6)]
        public string Password { get; init; }
    }
}
