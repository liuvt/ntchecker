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
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ntchecker.Services;
public class AuthenService : AuthenticationStateProvider, IAuthenService
{
    #region Constructor and Parameter
    private readonly HttpClient httpClient;
    // Using protected localstored of blazor not using JavaScript
    private readonly ProtectedLocalStorage protectedLocalStorage;
    private readonly IJSRuntime js;

    //Key localStorage
    private string key = "token";
    //Anonymous authentication state
    private AuthenticationState Anonymous =>
        new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));
    public AuthenService(HttpClient _httpClient, ProtectedLocalStorage _protectedLocalStorage, IJSRuntime _js)
    {
        this.httpClient = _httpClient;
        this.protectedLocalStorage = _protectedLocalStorage;
        this.js = _js;

        // Khởi động tự check lại sau khi JS Runtime ready
        _ = InitializeAsync();
    }
    private async Task InitializeAsync()
    {
        try
        {
            // Chờ đến khi JS Runtime khả dụng (khoảng vài ms)
            while (!js.IsJSRuntimeAvailable()) //Hàm kiểm tra trong Extensions
            {
                await Task.Delay(100); // Delay 100ms rồi kiểm tra lại
            }

            Console.WriteLine("[Info] JSRuntime đã sẵn sàng. Bắt đầu refresh AuthenticationState.");

            // Khi JS đã sẵn sàng => trigger lại AuthenticationStateChanged
            var authState = await GetAuthenticationStateInternalAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] InitializeAsync: {ex.Message}");
        }
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
                await protectedLocalStorage.SetAsync(key, token);

                // Gửi trạng thái xác thực đăng nhập 
                NotifyAuthenticationStateChanged(Task.FromResult(await BuildAuthenticationState(token)));

                return token;
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
            await protectedLocalStorage.DeleteAsync(key);

            // Gửi trạng thái xác thực đăng xuất 
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

            // Kiểm tra user.Identity có null hay không và trả về trạng thái xác thực
            return user.Identity?.IsAuthenticated ?? false;
        }
        catch (Exception ex)
        {
            // Log lỗi và trả về false nếu có sự cố
            Console.WriteLine($"Error checking authentication state: {ex.Message}");
            return false;
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
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!js.IsJSRuntimeAvailable()) //Hàm kiểm tra trong Extensions())
        {
            Console.WriteLine("[Info] JSRuntime chưa sẵn sàng (prerendering). Trả Anonymous.");
            return Anonymous;
        }

        return await GetAuthenticationStateInternalAsync();
    }

    /*
        Authentication
        - Get token in localStorage by key
        - Check token by ValidationToken(): bool
        - return BuildAuthenticationState(token)
    */
    private async Task<AuthenticationState> GetAuthenticationStateInternalAsync()
    {
        try
        {
            var result = await protectedLocalStorage.GetAsync<string>("token");
            var token = result.Success ? result.Value?.Replace("\"", "") : null;

            if (!ValidateToken(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
                return Anonymous;
            }

            return await BuildAuthenticationState(token);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"[Error] Đọc localStorage thất bại: {ex.Message}");
            return Anonymous;
        }
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
        try
        {
            //Lấy token từ localstorage vào chuyển đổi token mặt định
            var token = localStorageToken;

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            /* Lấy dữ liệu chuyển đổi từ Token sang các cập [Key:Value]
            var _user = state.User;
            var id = _user.Claims.Where(c => c.Type == "id").FirstOrDefault().Value;
            var name = _user.Claims.Where(c => c.Type == "name").FirstOrDefault().Value;
            Console.WriteLine($"id: {id}");
            Console.WriteLine($"name: {name}");
             */

            return state;
        }
        catch
        {
            Console.WriteLine($"ParseClaimsFromJwt token error");

            // Nếu có lỗi => return Anonymous
            return Anonymous;
        }
    }

    //Chuyển Token thành cặp [Key:Value]
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JWT: {ex.Message}");
            return Enumerable.Empty<Claim>();
        }
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
        // Kiểm tra rỗng
        if (string.IsNullOrEmpty(token))
            return false;

        // Kiểm tra token đọc được
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
            return false;

        // Đọc chuỗi
        var jwtToken = handler.ReadJwtToken(token);

        // Kiểm tra thời gian hết hạn
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        Console.WriteLine($"expClaim JWT: {expClaim}");

        if (expClaim != null)
        {
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim));
            if (expTime < DateTimeOffset.UtcNow)
            {
                Console.WriteLine("Token expired");
                return false;
            }
        }

        return true;
    }
    #endregion

}