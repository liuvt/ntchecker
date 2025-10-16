using System.Net;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Libraries.Entities;
using System.Net.Http.Json;

namespace TaxiNT.Client.Services;
public class BillCheckService : IBillCheckService
{
    private readonly HttpClient httpClient;

    //Constructor
    public BillCheckService(HttpClient _httpClient)
    {
        this.httpClient = _httpClient;
    }

    public async Task<ShiftWorkDto> Get(string userId, string? date)
    {
        try
        {
            HttpResponseMessage response;
            if (string.IsNullOrWhiteSpace(date))
                response = await httpClient.GetAsync($"api/ShiftWork?userId={userId}");
            else
                response = await httpClient.GetAsync($"api/ShiftWork?userId={userId}&date={date}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    return new ShiftWorkDto();

                var result = await response.Content.ReadFromJsonAsync<ShiftWorkDto>();

                if (result == null)
                    return new ShiftWorkDto();


                return result;
            }

            var error = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
        }
        catch (Exception ex)
        {
            // Có thể log ex ở đây
            throw new HttpRequestException($"Lỗi không load được data tư server --{ex}");
        }
    }
}
