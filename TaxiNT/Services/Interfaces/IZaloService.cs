using TaxiNT.Libraries.Models.Webhooks;

namespace TaxiNT.Services.Interfaces
{
    public interface IZaloService
    {
        Task<bool> AppendUserMessageTextAsync(UserMessageText model);
        Task<bool> AppendUserSendLocationAsync(UserMessageLocation model);
        Task<bool> AppendUserTextLocationAsync(ZaloWebhookEvent model);
    }
    
}
