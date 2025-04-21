using Microsoft.JSInterop;

namespace ntchecker.Extensions;
public static class JsRuntimeExtension
{
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
