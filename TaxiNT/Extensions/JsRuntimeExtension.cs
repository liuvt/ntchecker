using Microsoft.JSInterop;

namespace TaxiNT.Extensions;

//Hiện không sử dụng, sử dụng BlazorLocalStorage thay thế => mã hóa value
public static class JsRuntimeExtension
{
    // Kiểm tra trạng thái JS runtime đã sẳn sàng hay chưa
    public static bool IsJSRuntimeAvailable(this IJSRuntime jsRuntime) => jsRuntime is not null && jsRuntime is not IJSInProcessRuntime;
    /*
       Local Storage manager
       - GET/SET/REMOVE
    */
    public static ValueTask<string> ltvGetLocalStorage(this IJSRuntime jS, string key)
        => jS.InvokeAsync<string>($"localStorage.getItem", key);

    public static ValueTask ltvSetLocalStorage(this IJSRuntime jS, string key, string content)
        => jS.InvokeVoidAsync($"localStorage.setItem", key, content);

    public static ValueTask ltvRemoveLocalStorage(this IJSRuntime jS, string key)
        => jS.InvokeVoidAsync($"localStorage.removeItem", key);

    /*
        Xữ lý IFrame
    */
    public static ValueTask<int> ltvGetVideoIdFromIframe(this IJSRuntime jS)
        => jS.InvokeAsync<int>($"(document.getElementById('iframe_youtube').src).split('=',4)[3];");

    
}
