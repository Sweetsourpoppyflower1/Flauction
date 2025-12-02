namespace Flauction.DTOs.Output.ModelDTOs
{
    public class AuctionDTO
    {
        public int AuctionId { get; set; }
        public string AuctionMasterName { get; set; } 
        public string PlantName { get; set; }       
        public string WinnerCompanyName { get; set; } 
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal StartPrice { get; set; }
        public decimal MinimumPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }
}