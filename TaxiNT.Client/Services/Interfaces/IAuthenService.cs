using Microsoft.AspNetCore.Components.Authorization;
using TaxiNT.Libraries.Entities;

namespace TaxiNT.Client.Services.Interfaces;

public interface IAuthenService
{
    Task Login(GGSUserLoginDto model);
    Task LogOut();

    GGSUserTokenClaimDto UserClaimToken { get; }
}
