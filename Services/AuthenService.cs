using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ntchecker.Data.Entities;
using ntchecker.Extensions;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using ntchecker.Services.Interfaces;

namespace ntchecker.Services;
public class AuthenService : AuthenticationStateProvider, IAuthenService
{
    #region Constructor and Parameter
    private readonly HttpClient httpClient;
    //JavaScript
    private readonly IJSRuntime jS;
    //Key localStorage
    private string key = "_identityApp";
    //Anonymous authentication state
    private AuthenticationState Anonymous =>
        new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));
    public AuthenService(HttpClient _httpClient, IJSRuntime _jS)
    {
        this.httpClient = _httpClient;
        this.jS = _jS;
    }
    #endregion

    /*
        Login
        - Get API Login Controller by httpClientFactory
        - Set Token to LocalStorage
        - Call BuildAuthenticationState(token) to check state login
    */
    public async Task<string> Login(LoginUserDto login)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync<LoginUserDto>("api/Authen/Login", login);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return string.Empty;
                }

                //Lấy token từ API đăng nhập
                var token = await response.Content.ReadAsStringAsync();

                //Lưu token vào localStorage
                await jS.ltvSetLocalStorage(key, token);

                //Kiểm tra trạng thái xác thực
                var state = await BuildAuthenticationState(token);

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                //throw new Exception($"Http status: {response.StatusCode}. Message: {message}");
                throw new Exception(message);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /*
      Log Out
      - Remove token in localstorage
      - BuildAuthenticationState check state
  */
    public async Task LogOut()
    {
        try
        {
            await jS.ltvRemoveLocalStorage(key);

            //Kiểm tra trạng thái sau khi đăng nhập
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }
        catch (System.Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    /* Kiểm tra trạng thái đăng nhập trả về True or false */
    public async Task<bool> CheckAuthenState()
    {
        try
        {
            var authState = await GetAuthenticationStateAsync();
            var user = authState.User;
            return user.Identity.IsAuthenticated;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /* Xem trạng thái đăng nhập của User */
    public async Task<AuthenticationState> GetAuthenState() => await GetAuthenticationStateAsync();

    public async Task<bool> Register(LoginUserDto register)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync<LoginUserDto>("api/Authen/Register", register);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return false;
                }

                return true;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                //throw new Exception($"Http status: {response.StatusCode}. Message: {message}");
                throw new Exception(message);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #region Authentication State
    /*
        Authentication
        - Get token in localStorage by key
        - Check token by ValidationToken(): bool
        - return BuildAuthenticationState(token)
    */
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //Lấy token từ LocalStorage
        var token = await jS.ltvGetLocalStorage(key);

        //Kiểm tra xem token 
        if (!ValidateToken(token))
        {
            return Anonymous;
        }


        //Build AuthenticationState
        return await BuildAuthenticationState(token);
    }

    /*
        Build authentication state
        - Check authorization by token
        - Create ParseClaimsFromJwt get claims
        - Get Notify authentication state
        - return authenticationstate
    */
    private async Task<AuthenticationState> BuildAuthenticationState(string localStorageToken)
    {
        //Lấy token từ localstorage vào chuyển đổi token mặt định
        var token = localStorageToken.Replace("\"", "");

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        /* Lấy dữ liệu chuyển đổi từ Token sang các cập [Key:Value]
        var _user = state.User;
        var ObjectIdentifier = _user.Claims.Where(c => c.Type == "ObjectIdentifier").FirstOrDefault().Value;
        */

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    //Chuyển Token thành cặp [Key:Value]
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    //Parse Base64 Without Padding
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    //Validate
    private bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        //Check can read token
        var handler = new JwtSecurityTokenHandler();
        bool readToken = handler.CanReadToken(token.Replace("\"", ""));
        return readToken;
    }
    #endregion

}