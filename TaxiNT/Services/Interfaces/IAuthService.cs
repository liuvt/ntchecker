using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;

public interface IAuthService
{
    Task<GGSUser> GetGGSUser(string msnv);
    Task<GGSUser> GGSLogin(GGSUserLoginDto model);
}
