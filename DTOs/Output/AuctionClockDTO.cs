namespace Flauction.DTOs.Output
{
    public class AuctionClockDTO
    {
        public int AuctionClockId { get; set; }
        public int AuctionId { get; set; }
        // public string AuctionMasterName { get; set; }
        // public string PlantName { get; set; }
        public decimal TickIntervalSeconds { get; set; }
        public decimal DecrementAmount { get; set; }
        public int FinalCallSeconds { get; set; }
    }
}