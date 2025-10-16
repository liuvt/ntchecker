namespace TaxiNT.Libraries.Entities;
public class CheckerDetailDto
{
    public string userId { get; set; } = string.Empty;
    public RevenueDto? revenue { get; set; }
    public List<ContractDto>? contracts { get; set; }
    public List<TimepieceDto>? timepieces { get; set; }
    public int? countContract { get; set; }
    public decimal? TotalPriceContract { get; set; }
    public int? countTimepices { get; set; }
    public decimal? TotalPriceTimepices { get; set; }
}