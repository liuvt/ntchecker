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

        // 1) GET: Đễ test trên browser và AppSheet "go to website"
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

				string finalUrl = string.IsNullOrWhiteSpace(date) 
					? $"https://tienichtaixe.io.vn/checkers/{encrypted}" 
					: $"https://tienichtaixe.io.vn/checkers/{encrypted}/{Uri.EscapeDataString(date)}";

				//return Redirect(finalUrl);
                return Content($$"""
                                    <!DOCTYPE html>
                                    <html lang="vi">
                                    <head>
                                        <meta charset="utf-8">
                                        <title>Đang chuyển hướng...</title>

                                        <script src="https://cdn.tailwindcss.com"></script>

                                        <script>
                                            setTimeout(() => {
                                                window.location.replace('{{finalUrl}}');
                                            }, 1500);
                                        </script>
                                    </head>
                                    <body class="bg-slate-50 min-h-screen flex items-center justify-center">

                                        <div class="text-center">

                                            <div class="flex justify-center mb-6">
                                                <div class="w-16 h-16 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin"></div>
                                            </div>

                                            <h1 class="text-xl font-semibold text-slate-800">
                                                Đang tải dữ liệu
                                            </h1>

                                            <p class="mt-2 text-slate-500">
                                                Vui lòng chờ trong giây lát...
                                            </p>

                                        </div>

                                    </body>
                                    </html>
                                    """, "text/html");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OpenUser error. Id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // 3) GET: AppSheet mở URL này, backend mã hóa rồi redirect luôn sang link cuối
        // Ví dụ:
        // /api/crypto/open-user?id=Nguyen%20Van%20A%20-%20NV001
        // Chuyển hướng sang /checkers/{encrypted} để AppSheet mở ra trang checkers với id đã được mã hóa trong URL
        [HttpGet("open-user-salary")]
        public IActionResult OpenUserSalary([FromQuery] string id, string? date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("id is required.");

                var encrypted = CryptographyAESExtension.Encrypt(id);

                string finalUrl = string.IsNullOrWhiteSpace(date)
                    ? $"https://tienichtaixe.io.vn/salary/{encrypted}"
                    : $"https://tienichtaixe.io.vn/salary/{encrypted}/{Uri.EscapeDataString(date)}";

                return Content($$"""
                                <!DOCTYPE html>
                                <html lang="vi">
                                <head>
                                    <meta charset="utf-8">
                                    <meta name="viewport" content="width=device-width, initial-scale=1">
                                    <title>Đang tải bảng lương...</title>

                                    <script src="https://cdn.tailwindcss.com"></script>

                                    <script>
                                        setTimeout(() => {
                                            window.location.replace('{{finalUrl}}');
                                        }, 1200);
                                    </script>
                                </head>

                                <body class="bg-slate-100 min-h-screen flex items-center justify-center">

                                    <div class="text-center">

                                        <div class="flex justify-center">
                                            <div class="h-16 w-16 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin"></div>
                                        </div>

                                        <h2 class="mt-6 text-xl font-semibold text-slate-800">
                                            Đang tải bảng lương
                                        </h2>

                                        <p class="mt-2 text-slate-500">
                                            Vui lòng chờ trong giây lát...
                                        </p>

                                    </div>

                                </body>
                                </html>
                                """, "text/html");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "OpenUserSalary error. Id: {Id}", id);
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
