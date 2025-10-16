using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaxiNT.Libraries.Models.Webhooks;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ZaloController : ControllerBase
{
    //Get API Server
    private readonly ILogger<ZaloController> logger;
    private readonly IConfiguration configuration;
    private readonly IZaloService zaloService;
    public ZaloController(ILogger<ZaloController> _logger, IConfiguration _configuration, IZaloService _zaloService)
    {
        this.logger = _logger;
        this.configuration = _configuration;
        this.zaloService = _zaloService;
    }

    [HttpPost("zalo-webhook")]
    public async Task<ActionResult<string>> Post()
    {
        try
        {
            var rawBody = await new StreamReader(Request.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawBody))
            {
                logger.LogWarning("Webhook Zalo nhận body rỗng!");
                return Ok(new { message = "empty_body" });
            }
            var webhookEvent = JsonSerializer.Deserialize<ZaloWebhookEvent>(rawBody);
            logger.LogInformation("Webhook Zalo nhận: {rawBody}", rawBody);
            await zaloService.AppendUserTextLocationAsync(webhookEvent);

            return Ok(new { message = "success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xử lý webhook Zalo");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error: " + ex.Message);
        }
    }

    /* Chia từng phần để dễ quản lý và bảo trì
    [HttpPost("zalo-webhook")]
    public async Task<ActionResult<string>> Post()
    {
        try
        {
            var rawBody = await new StreamReader(Request.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawBody))
            {
                logger.LogWarning("Webhook Zalo nhận body rỗng!");
                return Ok(new { message = "empty_body" });
            }
            var webhookEvent = JsonSerializer.Deserialize<ZaloWebhookEvent>(rawBody);
            logger.LogInformation("Webhook Zalo nhận: {rawBody}", rawBody);

            if (webhookEvent == null)
                return BadRequest("Invalid webhook payload");

            switch (webhookEvent.EventName)
            {
                case "user_send_location":
                    var locationMsg = webhookEvent.Message?.Deserialize<UserMessageLocation>();
                    if (locationMsg != null && locationMsg.Attachments.Count > 0)
                    {
                        var coords = locationMsg.Attachments[0].Payload.Coordinates;
                        logger.LogInformation("User gửi location: Lat={lat}, Lng={lng}, Text={text}",
                            coords.Latitude, coords.Longitude, locationMsg.Text);
                        await zaloService.AppendUserSendLocationAsync(locationMsg);
                    }
                    break;

                case "user_send_text":
                    var textMsg = webhookEvent.Message?.Deserialize<UserMessageText>();
                    if (textMsg != null)
                    {
                        await zaloService.AppendUserMessageTextAsync(textMsg);
                    }
                    break;

                default:
                    logger.LogWarning("Sự kiện chưa xử lý: {event}", webhookEvent.EventName);
                    break;
            }

            return Ok(new { message = "success" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi xử lý webhook Zalo");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error: " + ex.Message);
        }
    }
    */

}