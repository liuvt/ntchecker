using Microsoft.AspNetCore.Mvc;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalaryController : ControllerBase
{
    //Get API Server
    private readonly ISalaryAPIService context;
    private readonly ILogger<SalaryController> logger;
    public SalaryController(ISalaryAPIService _context, ILogger<SalaryController> _logger)
    {
        this.context = _context;
        this.logger = _logger;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetSalary(string userId)
    {
        try
        {
            var result = await context.GetSalary(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetsRevenueDetail");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}/Details")]
    public async Task<IActionResult> GetSalaryDetails(string userId)
    {
        try
        {
            var result = await context.GetSalaryDetails(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetsRevenueDetail");
            return StatusCode(500, "Internal server error");
        }
    }

}
