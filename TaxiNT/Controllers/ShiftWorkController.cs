using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.MapperModels;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftWorkController : ControllerBase
    {
        //Get API Server
        private readonly IShiftWorkService context;
        private readonly ILogger<ShiftWorkController> logger;
        public ShiftWorkController(IShiftWorkService _context, ILogger<ShiftWorkController> _logger)
        {
            this.context = _context;
            this.logger = _logger;
        }

        [HttpPost("upsert-daily")]
        public async Task<IActionResult> UpsertShiftWorkDaily([FromBody] ShiftWorkDailySyncDto data)
        {
            try
            {
                var result = await context.UpsertShiftWorkDailyAsync(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error limit get api GoogleSheet");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets(string userId, string? date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest("UserId is required.");

                ///Nếu như date null thì lấy ngày hiện tại -1 để thu tiền phiếu ngày hôm qua
                //Bắt trạng thái Daily, nếu date rỗng → lấy ngày hôm qua
                var vnNow = DateTime.UtcNow.AddHours(7); //Múi giờ Việt Nam cộng thêm 7 tiếng, tức là múi giờ Việt Nam (UTC+7).
                var targetDate = string.IsNullOrWhiteSpace(date)
                    ? vnNow.AddDays(-1).ToString("yyyy-MM-dd")
                    : date;

                Console.WriteLine($"Target Date: {vnNow.AddDays(-1).ToString("yyyy-MM-dd")}");

                var result = await context.Gets(userId, targetDate);

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
}
