using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TaxiNT.Libraries.Models.Webhooks;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SwitchboardController : ControllerBase
{
    //Get API Server
    private readonly ISwitchboardService switchboardService;
    public SwitchboardController(ISwitchboardService _switchboardService)
    {
        this.switchboardService = _switchboardService;
    }

    [HttpGet("switchboard-webhook")] //https://taxinamthang.com/api/Switchboard/switchboard-webhook/
    public async Task<IActionResult> Receive([FromForm] Dictionary<string, string> formData)
    {
        if (!formData.ContainsKey("checksum"))
            return BadRequest("Missing checksum");

        string requestChecksum = formData["checksum"];
        formData.Remove("checksum");

        var sorted = formData.OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.Value);

        string secretKey = "ogrzCJRrbTVNOCJLzm6q";
        string json = JsonSerializer.Serialize(sorted);

        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(secretKey + json);
        var hashBytes = md5.ComputeHash(inputBytes);
        string checksum = string.Concat(hashBytes.Select(b => b.ToString("x2")));

        if (checksum != requestChecksum)
            return BadRequest("Invalid checksum");

        string action = sorted.GetValueOrDefault("action", "");
        string source = sorted.GetValueOrDefault("source", "");
        string destination = sorted.GetValueOrDefault("destination", "");
        string uniqueid = sorted.GetValueOrDefault("uniqueid", "");

        // tạo model để lưu
        var callLog = new CallLog
        {
            Action = action,
            Source = source,
            Destination = destination,
            UniqueId = uniqueid,
            CreatedAt = DateTime.UtcNow
        };

        // lưu Google Sheet
        await switchboardService.AppendCallLogAsync(callLog);

        // ghi log file (optional)
        string msg = $"[{action.ToUpper()}] {source} -> {destination}, UniqueId={uniqueid}";
        System.IO.File.AppendAllText("calls.log", msg + Environment.NewLine);

        return Ok(new { status = "success", action, source, destination, uniqueid });
    }
}