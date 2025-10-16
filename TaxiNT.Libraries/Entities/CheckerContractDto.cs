
namespace TaxiNT.Libraries.Entities;
public class ContractDto
{
    public string ctId { get; set; } = string.Empty;
    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    public decimal? ctAmount { get; set; }
    public string ctDefaultDistance { get; set; } = string.Empty;
    public string ctOverDistance { get; set; } = string.Empty;
    public decimal? ctSurcharge { get; set; }
    public decimal? ctPromotion { get; set; }
    public decimal? totalPrice { get; set; }
    public DateTime? createdAt { get; set; }
}

