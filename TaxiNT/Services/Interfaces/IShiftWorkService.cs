using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Services.Interfaces;
public interface IShiftWorkService
{
    Task<object> UpsertShiftWorkDailyAsync(ShiftWorkDailySyncDto data);
    Task<ShiftWorkDto> Gets(string userId, string date);
}
