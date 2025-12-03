using Microsoft.AspNetCore.Identity;

namespace Flauction.Models

{
    public class User : IdentityUser
    {
        public string email { get; set; }
        public string password { get; set; }

    }  
}
