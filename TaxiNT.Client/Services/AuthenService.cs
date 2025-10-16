using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using TaxiNT.Client.Extensions;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Client.Services;
public class AuthenService : AuthenticationStateProvider, IAuthenService
{
    #region Constructor and Parameter
    private readonly HttpClient httpClient;
    private readonly ILogger<AuthenService> logger;
    //JavaScript
    private readonly IJSRuntime jS;
    //Key localStorage
    private string key = "_taxintToken";
    //Save thông tin token
    public GGSUserTokenClaimDto UserClaimToken { get; private set; } = new GGSUserTokenClaimDto();
    //Anonymous authentication state
    private AuthenticationState Anonymous =>
        new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));

    //Constructor
    public AuthenService(HttpClient _httpClient, IJSRuntime _jS, ILogger<AuthenService> _logger)
    {
        this.httpClient = _httpClient;
        this.jS = _jS;
        this.logger = _logger;

        // Khởi động tự check lại sau khi JS Runtime ready
        _ = InitializeAsync();
    }
    #endregion

    #region Public Services
    public async Task Login(GGSUserLoginDto model)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync<GGSUserLoginDto>($"api/Auth/Login", model);

            if (response.IsSuccessStatusCode)
            {
                //Lấy token từ API đăng nhập
                var token = await response.Content.ReadAsStringAsync();

                //Lưu token vào localStorage
                await jS.SetFromLocalStorage(key, token);

                var state = await BuildAuthenticationState(token);
                NotifyAuthenticationStateChanged(Task.FromResult(state));

            }
            else
            {
                var mess = await response.Content.ReadAsStringAsync();
                throw new Exception(mess);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task LogOut()
    {
        try
        {
            await jS.RemoveFromLocalStorage(key);

            //Kiểm tra trạng thái sau khi đăng nhập
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }
        catch (System.Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region Delay JSRuntime
    // TaskCompletionSource để đợi JS Runtime sẵn sàng
    private TaskCompletionSource<bool> _authReady = new();

    public void NotifyJsRuntimeReady()
    {
        _authReady.TrySetResult(true);
    }
    private async Task InitializeAsync()
    {
        try
        {
            //Hàm kiểm tra trong IsJSRuntimeAvailable Extensions
            while (!jS.IsJSRuntimeAvailable())
            {
                logger.LogInformation($"JSRuntime not ready to prerendering");
                // Chờ JSRuntime sẵn sàng (sẽ do JS phía client gọi vào NotifyJsRuntimeReady)
                await _authReady.Task;
            }

            // Khi JS is ready => AuthenticationStateChanged
            var authState = await GetAuthenticationStateInternal();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        catch (Exception ex)
        {
            logger.LogError($"InitializeAsync: {ex.Message}");
        }
    }
    #endregion

    #region Authentication State
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (!jS.IsJSRuntimeAvailable())
            {
                logger.LogInformation("JSRuntime not ready to prerendering");
                return Anonymous;
            }

            return await GetAuthenticationStateInternal();
        }
        catch (Exception ex)
        {
            logger.LogError($"Authentication state internal: {ex.Message}");
            return Anonymous;
        }
    }
    private async Task<AuthenticationState> GetAuthenticationStateInternal()
    {
        try
        {
            //Lấy token từ LocalStorage
            var token = await jS.GetFromLocalStorage(key);

            if (!ValidateToken(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
                return Anonymous;
            }

            return await BuildAuthenticationState(token);
        }
        catch (Exception ex)
        {
            return Anonymous;
        }
    }

    private async Task<AuthenticationState> BuildAuthenticationState(string localStorageToken)
    {
        try
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
            // Lấy thông tin User từ Token lưu vào UserClaimToken
            var _user = state.User;
            UserClaimToken = new GGSUserTokenClaimDto
            {
                No = _user.Claims.FirstOrDefault(c => c.Type == "No.")?.Value ?? string.Empty,
                Username = _user.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? string.Empty,
                Area = _user.Claims.FirstOrDefault(c => c.Type == "area")?.Value ?? string.Empty,
                Name = _user.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
                JwtRegisteredClaimNames = _user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty
            };

            return state;
        }
        catch (Exception ex)
        {
            logger.LogError($"Building authentication state: {ex.Message}");
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
            logger.LogError($"Parsing claims JWT: {ex.Message}");
            return Enumerable.Empty<Claim>();
        }
    }

    //Parse Base64 Without Padding
    private byte[] ParseBase64WithoutPadding(string base64Url)
    {
        // Chuyển về Base64 chuẩn
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
            case 1: throw new Exception("Invalid Base64 string length");
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

        if (expClaim != null)
        {
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim));
            if (expTime < DateTimeOffset.UtcNow)
            {
                logger.LogError("Token expired");
                return false;
            }
        }

        return true;
    }
    #endregion
}
