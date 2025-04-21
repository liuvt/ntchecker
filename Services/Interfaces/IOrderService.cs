namespace ntchecker.Services.Interfaces;
public interface IOrderService
{
    Task<Data.Models.GGSheetModels.Revenue> GetRevenue(string userId);
    Task<Data.Models.GGSheetModels.Timepiece> GetTimepiece(string userId);
    Task<Data.Models.GGSheetModels.Contract> GetContract(string userId);
}
