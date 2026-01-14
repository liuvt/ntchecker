namespace TaxiNT.Services.Interfaces;

public interface IFcmSenderService
{
    Task SendToTokenAsync(string token, string title, string body, string link = "/");
}
