using Microsoft.JSInterop;
using System;

namespace RecipeLewis.Business;

public class MenuService
{
    private IJSRuntime jsRuntime;
    public MenuService(IJSRuntime JSRuntime)
    {
        jsRuntime = JSRuntime;
    }
    public bool IsAuthenticated { get; private set; }
    public bool IsDarkMode { get; private set; }

    public void SetIsAuthenticated(bool isAuthenticated)
    {
        IsAuthenticated = isAuthenticated;
        NotifyStateChanged();
    }

    public event Action OnChange; // event raised when changed

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task SetDarkMode(bool isDarkMode)
    {
        if (isDarkMode)
        {
            await jsRuntime.InvokeVoidAsync("QuillFunctions.loadDarkModeCss");
        }
        else
        {
            await jsRuntime.InvokeVoidAsync("QuillFunctions.loadLightModeCss");
        }
        IsDarkMode = isDarkMode;
        NotifyStateChanged();
    }
}