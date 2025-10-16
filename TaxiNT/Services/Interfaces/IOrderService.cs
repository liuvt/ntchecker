using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;
public interface IOrderService
{
    Task<Revenue> GetRevenue(string userId);
    Task<Timepiece> GetTimepiece(string userId);
    Task<GGSContract> GetContract(string userId);
}