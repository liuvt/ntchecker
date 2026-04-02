using Microsoft.AspNetCore.Mvc;
using TaxiNT.Extensions;

namespace TaxiNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly ILogger<CryptoController> logger;
        public CryptoController(ILogger<CryptoController> _logger)
        {
            this.logger = _logger;
        }

        // 1) GET: dễ test trên browser và AppSheet "go to website"
        // Ví dụ:
        // /api/crypto/encrypt-query?id=Nguyen%20Van%20A%20-%20NV001
        [HttpGet("encrypt-query")]
        public IActionResult EncryptByGet([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "id is required."
                    });
                }

                var encrypted = CryptographyAESExtension.Encrypt(id);

                return Ok(new
                {
                    success = true,
                    input = id,
                    result = encrypted,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EncryptByQuery error. Id: {Id}", id);

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // 3) GET: AppSheet mở URL này, backend mã hóa rồi redirect luôn sang link cuối
        // Ví dụ:
        // /api/crypto/open-user?id=Nguyen%20Van%20A%20-%20NV001
        // Chuyển hướng sang /checkers/{encrypted} để AppSheet mở ra trang checkers với id đã được mã hóa trong URL
        [HttpGet("open-user")]
        public IActionResult OpenUser([FromQuery] string id, string? date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("id is required.");

                var encrypted = CryptographyAESExtension.Encrypt(id);

                string finalUrl;

                if (string.IsNullOrWhiteSpace(date))
                {
                    finalUrl = $"/checkers/{encrypted}";
                }
                else
                {
                    finalUrl = $"/checkers/{encrypted}/{Uri.EscapeDataString(date)}";
                }

                return Redirect(finalUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OpenUser error. Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // 2) POST: gửi id trong body, trả về kết quả mã hóa, dễ test bằng Postman hoặc các công cụ tương tự
        [HttpPost("encrypt-query")]
        public IActionResult EncryptByQuery([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest(new
                    {
                        success = false,
                        message = "id is required."
                    });

                var encrypted = CryptographyAESExtension.Encrypt(id);

                return Ok(new
                {
                    success = true,
                    input = id,
                    result = encrypted
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EncryptByQuery error. Id: {Id}", id);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }

        // 2) POST: test bằng Postman hoặc AppSheet "webhook", gửi id trong query string, backend trả về JSON chứa kết quả đã mã hóa
        [HttpPost("decrypt-query")]
        public IActionResult DecryptByQuery([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest(new
                    {
                        success = false,
                        message = "id is required."
                    });

                var decrypted = CryptographyAESExtension.Decrypt(id);

                return Ok(new
                {
                    success = true,
                    input = id,
                    result = decrypted
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DecryptByQuery error. Id: {Id}", id);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }



    }
}
