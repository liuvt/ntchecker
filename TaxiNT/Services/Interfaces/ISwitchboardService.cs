using TaxiNT.Libraries.Models.Webhooks;

namespace TaxiNT.Services.Interfaces
{
    public interface ISwitchboardService
    {
        Task<bool> AppendCallLogAsync(CallLog model);
    }
    
}
