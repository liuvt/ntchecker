namespace TaxiNT.Libraries.Entities;

public class TimepieceDto
{
    public string userId { get; set; } = string.Empty;
    public string numberCar { get; set; } = string.Empty;
    public string tpTimeStart { get; set; } = string.Empty;
    public string tpTimeEnd { get; set; } = string.Empty;
    public string tpDistance { get; set; } = string.Empty;
    public decimal? tpPrice { get; set; } 
    public string tpPickUp { get; set; } = string.Empty;
    public string tpDropOut { get; set; } = string.Empty;
    public string tpType { get; set; } = string.Empty; //Loaị cuốc xe
    public string createdAt { get; set; } = string.Empty; //Ngày nợp tiền cuốc xe
}
