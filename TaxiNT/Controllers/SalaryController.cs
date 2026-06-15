using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Extensions;
using TaxiNT.Services.Interfaces;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
                formattedDate = FormatMonth.FormatSalaryMonth(date);

                if (formattedDate == null)
                {
                    return BadRequest(
                        "Invalid date format. Expected: yyyy-MM, MM-yyyy hoặc MM/yyyy."
                    );
                }
            }

            Console.WriteLine($"Keyword: {keyword}, Formatted Date: {formattedDate}");
            var result = await context.GetSalaryByUserId(keyword, formattedDate);

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

    #region Main
    // Lấy đầy đủ Salary + Details (1 API)
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetSalaryByUserId(string userId, [FromQuery] string? date = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("UserId is required.");

            //Khi truyền date 05-2026 hoặc 2026-05 thì vẫn có thể xử lý được, chỉ cần lấy 7 ký tự cuối là "2026-05", và format lại thành 05/2026 để query theo tháng
            string? formattedDate = null;
            if (!string.IsNullOrWhiteSpace(date))
            {
                formattedDate = FormatMonth.FormatSalaryMonth(date);

                if (formattedDate == null)
                {
                    return BadRequest(
                        "Invalid date format. Expected: yyyy-MM, MM-yyyy hoặc MM/yyyy."
                    );
                }
            }

            Console.WriteLine($"Keyword: {userId}, Formatted Date: {formattedDate}");

            var result = await context.GetSalaryByUserId(userId, formattedDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetSalaryFull");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("full-upsert")]
    public async Task<IActionResult> UpsertFullSalary([FromBody] List<SalaryUpsertRequest> requests)
    {
        try
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest(new { message = "Danh sách rỗng" });
            }

            var results = await context.UpsertSalary(requests);
            return Ok(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in UpsertFullSalary");
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Delete
    /// <summary>
    /// Xóa full nhiều bảng lương theo userId + salaryDate.
    /// Xóa theo thứ tự: SalaryDeductDetails -> SalaryDetails -> Salaries.
    /// </summary>
    [HttpDelete("delete-list")]
    public async Task<IActionResult> SalaryDeleteList([FromBody] List<SalaryDeleteRequest> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Danh sách yêu cầu xóa không được để trống"
            });
        }

        var result = await context.SalaryDeleteList(requests);

        return Ok(new
        {
            success = true,
            message = "Đã xử lý yêu cầu xóa danh sách lương",
            total = result.Count,
            successCount = result.Count(x => x.Success),
            failedCount = result.Count(x => !x.Success),
            data = result
        });
    }

    /// <summary>
    /// Xóa full bảng lương theo khu vực + tháng lương.
    /// Ví dụ: area = Rạch Giá, salaryDate = 05/2026.
    /// </summary>
    [HttpDelete("delete-by-area")]
    public async Task<IActionResult> DeleteSalaryByArea([FromBody] SalaryDeleteByAreaRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Request không hợp lệ"
            });
        }

        if (string.IsNullOrWhiteSpace(request.area))
        {
            return BadRequest(new
            {
                success = false,
                message = "area không được để trống"
            });
        }

        if (string.IsNullOrWhiteSpace(request.salaryDate))
        {
            return BadRequest(new
            {
                success = false,
                message = "salaryDate không được để trống"
            });
        }

        var deletedCount = await context.SalaryDeleteByAreaAndDate(request);

        return Ok(new
        {
            success = true,
            message = deletedCount > 0
                ? "Xóa dữ liệu lương theo khu vực thành công"
                : "Không tìm thấy dữ liệu lương phù hợp để xóa",
            area = request.area,
            salaryDate = request.salaryDate,
            deletedCount = deletedCount
        });
    }
    #endregion

    
}
