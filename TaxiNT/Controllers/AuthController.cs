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
public class AuthController : ControllerBase
{
    //Get API Server
    private readonly IAuthService context;
    private readonly ILogger<AuthController> logger;
    private readonly IConfiguration configuration;
    public AuthController(IAuthService _context, ILogger<AuthController> _logger, IConfiguration _configuration)
    {
        this.context = _context;
        this.logger = _logger;
        this.configuration = _configuration;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(GGSUserLoginDto model)
    {
        try
        {
            //Login
            var user = await this.context.GGSLogin(model);
            if (user == null)
                throw new Exception("Wrong Email or Password");

            var token = await this.CreateToken(user);

            return Ok(token);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                                                                "Error: " + ex.Message);
        }
    }

    /* Tạo token*/
    private async Task<string> CreateToken(GGSUser user)
    {
        try
        {
            //Thông tin User đưa vào Token
            var listClaims = new List<Claim>
                        {
                            new Claim("No.", user.so_tai),
                            new Claim("username", user.msnv),
                            new Claim("area", user.khu_vuc),
                            new Claim("name", user.hoten_msnv),
                            new Claim(JwtRegisteredClaimNames.Jti, user.id)
                        };

            //Khóa bí mật
            var autKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]
                ?? throw new InvalidOperationException("Can't found [Secret Key] in appsettings.json !")));

            //Tạo chữ ký với khóa bí mật
            var signCredentials = new SigningCredentials(autKey, SecurityAlgorithms.HmacSha512Signature);

            var autToken = new JwtSecurityToken(
                claims: listClaims, //Thông tin User
                issuer: configuration["JWT:ValidIssuer"], //In file appsetting.json
                audience: configuration["JWT:ValidAudience"], //In file appsetting.json
                expires: DateTime.Now.AddDays(30), //Thời gian tồn tại Token
                signingCredentials: signCredentials //Chữ ký
            );

            return new JwtSecurityTokenHandler().WriteToken(autToken);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

