using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FcmPushController : ControllerBase
{
    //Get API Server
    private readonly IFcmSenderService context;
    private readonly ILogger<FcmPushController> logger;
    private readonly IConfiguration configuration;
    public FcmPushController(IFcmSenderService _context, ILogger<FcmPushController> _logger, IConfiguration _configuration)
    {
        this.context = _context;
        this.logger = _logger;
        this.configuration = _configuration;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterTokenRequest req)
    {
        // Lưu req.Token theo user/device vào DB
        Console.WriteLine($"Received FCM Token: {req.Token}");
        return Ok();
    }

    public class RegisterTokenRequest
    {
        public string Token { get; set; } = "";
    }
}

