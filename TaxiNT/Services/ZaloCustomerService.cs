using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaxiNT.Libraries.Models.Zalos;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services
{
    public class ZaloCustomerService : IZaloCustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBotAuto = "https://api.smax.ai/public/bizs/namthanggroup/triggers/6854d195cb4d422efe074e54";
        public ZaloCustomerService()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task SendMessageToGroup(string groupzalo, string _value)
        {
            try
            {

                ZPayloads payload = new ZPayloads
                {
                    customer = new ZCustomer
                    {
                        pid = groupzalo,
                        page_pid = "zlw64692029372977703"
                    },
                    attrs = new List<ZItem>
                    {
                        new ZItem
                        {
                            name = "noi_dung_tin",
                            value = _value
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);

                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0cmlnZ2VyX2lkIjoiNjg1NGQxOTVjYjRkNDIyZWZlMDc0ZTU0IiwiaWF0IjoxNzUwMzg5MTQxLCJleHAiOjMxNzI5NDgzMTU0MX0.pl2fKpZlmqGgHGzxSfMR_Aaf4azHLU2U3VTgEKHby1o");

                HttpResponseMessage response = await _httpClient.PostAsync(_urlBotAuto, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"result: {result}");

            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                Console.WriteLine($"Error sending message to Zalo: {ex.Message}");
            }
        }
    }
}
