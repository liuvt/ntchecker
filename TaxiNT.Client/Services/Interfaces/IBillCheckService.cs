using TaxiNT.Libraries.Entities;

namespace TaxiNT.Client.Services.Interfaces;

public interface IBillCheckService
{
    Task<ShiftWorkDto> Get(string userId, string? date);
}
