using TaxiNT.Libraries.Extensions;

namespace TaxiNT.Libraries.Models.GGSheets;

public class Timepiece
{
    public string userId { get; set; } = string.Empty;
    public List<TimepieceDetail>? timepieces { get; set; }

    public string TotalPrice => timepieces?.ltvSumFieldValues<TimepieceDetail>(e => e.tpPrice);
    public int count => timepieces?.Count ?? 0;
}

public class TimepieceDetail
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); //Tự động
    public string numberCar { get; set; } = string.Empty;
    public string tpTimeStart { get; set; } = string.Empty;
    public string tpTimeEnd { get; set; } = string.Empty;
    public string tpDistance { get; set; } = string.Empty;
    public string tpPrice { get; set; } = string.Empty;
    public string tpPickUp { get; set; } = string.Empty;
    public string tpDropOut { get; set; } = string.Empty;
    public string tpType { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
    public string createdAt { get; set; } = string.Empty;
}
