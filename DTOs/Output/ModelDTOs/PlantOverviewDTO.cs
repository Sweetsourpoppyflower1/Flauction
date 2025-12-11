namespace Flauction.DTOs.Output.ModelDTOs
{
    public class PlantOverviewDTO
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string Supplier { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Form { get; set; }
        public string Quality { get; set; }
        public string MinStem { get; set; }
        public string StemsBunch { get; set; }
        public string Maturity { get; set; }
        public string Desc { get; set; }
        public string ImageUrl { get; set; }
        public string ImageAlt { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int? RemainingQuantity { get; set; }
    }
}
