using Microsoft.AspNetCore.Identity;
using ntchecker.Data.Entities;
using ntchecker.Data.Models;

namespace ntchecker.Repositories.Interfaces;

public interface IAuthenRepository
{
    //Login
    Task<AppUser> Login(LoginUserDto login);
    //Register
    Task<IdentityResult> Register(RegisterUserDto register);

    //Create Token
    Task<string> CreateToken(InfomationUserSaveInToken user);

    //Get Role name
    Task<string> GetRoleName(AppUser user);

    //Get me
    Task<AppUser> GetMe(string userId);

    //Edit 
    Task<IdentityResult> EditMe(EditUserDto models, string userId);
    //Delete
    Task<IdentityResult> DeleteMe(string userId);
    Task<IdentityResult> ChangeCurrentPassword(string userId, string oldPassword, string newPassword);
}