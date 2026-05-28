using Microsoft.AspNetCore.Mvc;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;   // Giữ cho các class khác nếu cần
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

}
