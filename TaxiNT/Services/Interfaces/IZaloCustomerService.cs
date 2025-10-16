namespace TaxiNT.Services.Interfaces
{
    public interface IZaloCustomerService
    {
        Task SendMessageToGroup(string groupzalo, string _value);
    }
}
