using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.DTOs.Output.LoginDTO
{
    public class LoginResponseDTO
    {
        public string Token { get; init; }
        public DateTime Expires { get; init; }
    }
}
