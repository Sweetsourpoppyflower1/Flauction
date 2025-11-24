using Flauction.DTOs.Output.ModelDTOs;

namespace Flauction.DTOs.Output.LoginDTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = "";
        public DateTime Expiration { get; set; }
        public AuctionMasterDTO AuctionMaster { get; set; } = null;
    }
}
