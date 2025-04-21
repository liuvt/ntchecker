using Microsoft.AspNetCore.Mvc;
using ntchecker.Data.Entities;
using ntchecker.Data.Models;
using ntchecker.Repositories.Interfaces;

namespace ntchecker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenController : ControllerBase
{
    //Get API Server
    private readonly IAuthenRepository context;
    private readonly ILogger<AuthenController> logger;
    public AuthenController(IAuthenRepository _context, ILogger<AuthenController> _logger)
    {
        this.context = _context;
        this.logger = _logger;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserDto register)
    {
        try
        {
            var result = await this.context.Register(register);
            if (!result.Succeeded) return Unauthorized();

            return Ok(result.Succeeded);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(LoginUserDto appLogin)
    {
        try
        {
            //Login
            var appUser = await this.context.Login(appLogin);
            if (appUser == null)
                throw new Exception("Wrong Email or Password");

            //Role
            var role = await this.context.GetRoleName(appUser);

            //Create token
            var userClaim = new InfomationUserSaveInToken()
            {
                id = appUser.Id ?? string.Empty,
                email = appUser.Email ?? string.Empty,
                name = $"{appUser.FirstName} {appUser.LastName}" ?? string.Empty,
                giveName = $"{appUser.FirstName} {appUser.LastName}" ?? string.Empty,
                userName = appUser.UserName ?? string.Empty,
                userRole = role,
                userGuiId = Guid.NewGuid().ToString()
            };

            var token = await this.context.CreateToken(userClaim);

            return Ok(token);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                                                                "Error: " + ex.Message);
        }
    }

}