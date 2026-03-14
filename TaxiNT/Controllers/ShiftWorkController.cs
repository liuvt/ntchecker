using Microsoft.AspNetCore.Mvc;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
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
                return result as IActionResult ?? Ok(result);
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

        [HttpPost("upsert-daily-by-id")]
        public async Task<IActionResult> UpsertShiftWorkDailyById([FromBody] ShiftWorkUpsertByIdDto data)
        {
            try
            {
                var result = await context.UpsertShiftWorkDailyByIdAsync(data);
                return result as IActionResult ?? Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error upsert shiftwork daily by id");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-area-createdAt")]
        public async Task<IActionResult> GetByAreaAndCreatedAt(string area, string createdAt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(area))
                    return BadRequest("Area is required.");

                ///Nếu như date null thì lấy ngày hiện tại -1 để thu tiền phiếu ngày hôm qua
                //Bắt trạng thái Daily, nếu date rỗng → lấy ngày hôm qua
                var vnNow = DateTime.UtcNow.AddHours(7); //Múi giờ Việt Nam cộng thêm 7 tiếng, tức là múi giờ Việt Nam (UTC+7).
                var targetDate = string.IsNullOrWhiteSpace(createdAt)
                    ? vnNow.AddDays(-1).ToString("yyyy-MM-dd")
                    : createdAt;

                var result = await context.GetShiftWorkDtosByAreaAndCreatedAtAsync(area, targetDate);

                if (result == null || result.Count == 0)
                    return NoContent();

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error get ShiftWorkDto by Area + createdAt");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("get-crypto")]
        public async Task<IActionResult> GetsCrytoAES(string cryptoAES, string? date)
        {
            try
            {
                var userId = CryptographyAESExtension.Decrypt(cryptoAES);
                // ví dụ: "Nguyễn   Văn A - NV001"
                string keyword = SearchNormalizer.Normalize(userId);
                // => "NGUYỄN VĂN A - NV001"


                if (string.IsNullOrWhiteSpace(keyword))
                    return BadRequest("UserId is required.");

                ///Nếu như date null thì lấy ngày hiện tại -1 để thu tiền phiếu ngày hôm qua
                //Bắt trạng thái Daily, nếu date rỗng → lấy ngày hôm qua
                var vnNow = DateTime.UtcNow.AddHours(7); //Múi giờ Việt Nam cộng thêm 7 tiếng, tức là múi giờ Việt Nam (UTC+7).
                var targetDate = string.IsNullOrWhiteSpace(date)
                    ? vnNow.AddDays(-1).ToString("yyyy-MM-dd")
                    : date;

                var result = await context.Gets(keyword, targetDate);

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
