namespace TaxiNT.Libraries.Entities;

public class CheckerDto
{
    public string userId { get; set; } = string.Empty;
    public decimal? revenueByMonth { get; set; }
    public decimal? revenueByDate { get; set; }
    public decimal? totalPrice { get; set; }
    public string createdAt { get; set; } = string.Empty;
}
