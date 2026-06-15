using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;   // Salary & SalaryDetails đã chuyển về namespace này

namespace TaxiNT.Client.Services;
public class SalaryService : ISalaryService
{
    private readonly HttpClient httpClient;

    //Constructor
    public SalaryService(HttpClient _httpClient)
    {
        this.httpClient = _httpClient;
    }

    public async Task<SalaryFullResponseDto> GetSalaryFull(string userId, string? date = null)
    {
        Console.WriteLine($"[SalaryService] GetSalaryFull được gọi");
        Console.WriteLine($"[SalaryService] userId nhận được: '{userId}'");
        Console.WriteLine($"[SalaryService] date nhận được: '{date ?? "null"}'");

        // Encode userId để tránh lỗi URL khi có khoảng trắng hoặc ký tự đặc biệt (ví dụ: "LÊ QUỐC MINH - AG0249")
        string encodedUserId = Uri.EscapeDataString(userId);
        string url = $"api/Salary/{encodedUserId}/full";

        if (!string.IsNullOrWhiteSpace(date))
        {
            url += $"?date={Uri.EscapeDataString(date.Trim())}";
        }

        Console.WriteLine($"[SalaryService] URL gọi: {url}");

        var response = await httpClient.GetAsync(url);
        Console.WriteLine($"[SalaryService] HTTP Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[SalaryService] Lỗi từ API: {errorContent}");

            throw new HttpRequestException(
                $"Gọi API thất bại ({response.StatusCode}). Chi tiết: {errorContent}");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = await response.Content.ReadFromJsonAsync<SalaryFullResponseDto>(options);
        Console.WriteLine($"[SalaryService] Deserialize thành công. Có Salary: {result?.Salary != null}, Số Details: {result?.Details?.Count ?? 0}");

        return result ?? new SalaryFullResponseDto();
    }
}
