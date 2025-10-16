using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Client.Services;
public class SalaryService : ISalaryService
{
    private readonly HttpClient httpClient;

    //Constructor
    public SalaryService(HttpClient _httpClient)
    {
        this.httpClient = _httpClient;
    }

    public async Task<Salary> GetSalary(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new Salary();
            var response = await httpClient.GetAsync($"api/Salary/{userId}");

            if (response.StatusCode == HttpStatusCode.NoContent)
                return new Salary();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Salary>();
                return result ?? new Salary();
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
        }
        catch (Exception ex)
        {
            // TODO: Log exception nếu cần
            throw new ApplicationException("Lỗi khi gọi API lấy lương", ex);
        }
    }

    public async Task<List<SalaryDetails>> GetSalaryDetails(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<SalaryDetails>();

            Console.WriteLine($"Encoded UserId: {userId}");
            var response = await httpClient.GetAsync($"api/Salary/{userId}/Details");
            Console.WriteLine($"Encoded UserId: {httpClient.BaseAddress}");


            if (response.StatusCode == HttpStatusCode.NoContent)
                return new List<SalaryDetails>();


            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<SalaryDetails>>();
                return result ?? new List<SalaryDetails>();

            }

            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API Error: {response.StatusCode} - {error}");
        }
        catch (Exception ex)
        {
            // TODO: Log exception nếu cần
            throw new ApplicationException("Lỗi khi gọi API lấy lương", ex);
        }
    }
}
