using Microsoft.AspNetCore.Identity;

namespace Flauction.Models
{
    public class User : IdentityUser
    {
        // Use IdentityUser.Email and IdentityUser.PasswordHash instead of duplicating properties.
    }
}
