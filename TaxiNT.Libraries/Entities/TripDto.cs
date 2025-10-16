namespace TaxiNT.Libraries.Entities
{
    public class TripDto
    {
        public string Id { get; set; } = string.Empty;
        public string NumberCar { get; set; } = string.Empty;
        public DateTime? tpTimeStart { get; set; }
        public DateTime? tpTimeEnd { get; set; }
        public double? tpDistance { get; set; }
        public decimal? tpPrice { get; set; }
        public string tpPickUp { get; set; } = string.Empty;
        public string tpDropOut { get; set; } = string.Empty;
        public string tpType { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; }
    }
}
