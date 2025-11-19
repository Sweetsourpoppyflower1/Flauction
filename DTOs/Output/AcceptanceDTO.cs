namespace Flauction.DTOs.Output
{
    public class AcceptanceDTO
    {
        public int AcceptanceId { get; set; }
        ////public string PlantName { get; set; }
        public string CompanyName { get; set; }
        public int AuctionLotId { get; set; }
        public int TickNumber { get; set; }
        public decimal AcceptedPrice { get; set; }
        public int AcceptedQuantity { get; set; }
        public DateTime AcceptanceTime { get; set; }
    }
}
