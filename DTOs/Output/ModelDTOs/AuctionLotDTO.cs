namespace Flauction.DTOs.Output.ModelDTOs
{
    public class AuctionLotDTO
    {
        public int AuctionLotId { get; set; }
        //public string AuctionMasterName { get; set; }
        //public string PlantName { get; set; }
        public string MediaID { get; set; }
        public string MediaAltText { get; set; }
        public int UnitPerContainer { get; set; }
        public int ContainersInLot { get; set; }
        public int MinPickup { get; set; }
        public int Fustcode { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
    }
}