using Microsoft.AspNetCore.Components.Authorization;
using ntchecker.Data.Entities;

namespace ntchecker.Services.Interfaces
{
    public interface IAuthenService
    {
        Task<string> Login(LoginUserDto login);
        Task<bool> Register(LoginUserDto register);

        // Authen
        Task LogOut();
        Task<bool> CheckAuthenState();
        Task<AuthenticationState> GetAuthenState();
    }
}
