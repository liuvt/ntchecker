using TaxiNT.Services.Interfaces;
namespace TaxiNT.Services;
public class FcmSenderService : IFcmSenderService
{
    public async Task SendToTokenAsync(string token, string title, string body, string link = "/")
    {
        var message = new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Webpush = new WebpushConfig
            {
                FcmOptions = new WebpushFcmOptions { Link = link }
            }
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}
