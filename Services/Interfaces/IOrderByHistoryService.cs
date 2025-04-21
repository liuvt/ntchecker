namespace ntchecker.Services.Interfaces;
public interface IOrderByHistoryService
{
    Task<Data.Models.GGSheetModels.Revenue> GetRevenue(string userId, string date);
    Task<Data.Models.GGSheetModels.Timepiece> GetTimepiece(string userId, string date);
    Task<Data.Models.GGSheetModels.Contract> GetContract(string userId, string date);
}
