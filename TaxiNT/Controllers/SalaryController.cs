using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalaryController : ControllerBase
{
    //Get API Server
    private readonly ISalaryServer context;
    private readonly ILogger<SalaryController> logger;
    public SalaryController(ISalaryServer _context, ILogger<SalaryController> _logger)
    {
        this.context = _context;
        this.logger = _logger;
    }

    // Lấy đầy đủ Salary + Details (1 API)
    [HttpGet("{userId}/full")]
    public async Task<IActionResult> GetSalaryFull(string userId, [FromQuery] string? date = null)
    {
        try
        {
            var result = await context.GetSalaryFull(userId, date);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetSalaryFull");
            return StatusCode(500, "Internal server error");
        }
    }

    // 3. Upsert toàn bộ (list Salary + Details) - Hỗ trợ bulk từ Google Apps Script
    [HttpPost("full-upsert")]
    public async Task<IActionResult> UpsertFullSalary([FromBody] List<SalaryFullUpsertRequest> requests)
    {
        try
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest(new { message = "Danh sách rỗng" });
            }

            var results = await context.UpsertFullSalary(requests);
            return Ok(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in UpsertFullSalary");
            return StatusCode(500, "Internal server error");
        }
    }

    // Lấy ca làm việc theo userId và date (tháng 05/2026), với userId được mã hóa AES
    [HttpGet("get-salary-crypto")]
    public async Task<IActionResult> GetSalaryCrytoAES(string cryptoAES, [FromQuery] string? date = null)
    {
        try
        {
            //Dịch ngược AES
            var userId = CryptographyAESExtension.Decrypt(cryptoAES);

            // ví dụ: "Nguyễn   Văn A - NV001"
            // => "NGUYỄN VĂN A - NV001"
            string keyword = SearchNormalizer.Normalize(userId);


            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("UserId is required.");

            //Khi truyền date 05-2026 hoặc 2026-05 thì vẫn có thể xử lý được, chỉ cần lấy 7 ký tự cuối là "2026-05", và format lại thành 05/2026 để query theo tháng
            string? formattedDate = null;

            if (!string.IsNullOrWhiteSpace(date))
            {
                //Regex này sẽ kiểm tra xem date có định dạng "MM-yyyy" hoặc "yyyy-MM" hay không. Chỉ lấy phần tháng và năm, bất kể người dùng nhập theo định dạng nào.
                var match = Regex.Match(date.Trim(), @"(\d{4}-(0[1-9]|1[0-2])|(0[1-9]|1[0-2])-\d{4})$");

                if (!match.Success)
                    return BadRequest("Invalid date format. Expected format: MM-yyyy or yyyy-MM.");

                var value = match.Value;

                // Nếu định dạng là "yyyy-MM", chuyển thành "MM/yyyy"
                if (Regex.IsMatch(value, @"^\d{4}-\d{2}$"))
                {
                    var parts = value.Split('-');
                    formattedDate = $"{parts[1]}/{parts[0]}";
                }
                else
                {
                    // Nếu định dạng là "MM-yyyy", chuyển thành "MM/yyyy"
                    var parts = value.Split('-');
                    formattedDate = $"{parts[0]}/{parts[1]}";
                }
            }

            Console.WriteLine($"Keyword: {keyword}, Formatted Date: {formattedDate}");
            var result = await context.GetSalaryFull(keyword, formattedDate);

            if (result == null)
                return NoContent(); // 204: Không có dữ liệu

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
