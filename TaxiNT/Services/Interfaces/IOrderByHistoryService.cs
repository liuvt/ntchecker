using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;
public interface IOrderByHistoryService
{
    // Checker
    Task<List<RevenueDetail>> GetsRevenueDetail(string userId);
}
