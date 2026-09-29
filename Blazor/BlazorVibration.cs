using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace Avae.Essentials;

/// <summary>Vibration backed by navigator.vibrate (mobile browsers; no-ops when unsupported by hardware).</summary>
public class BlazorVibration(IJSRuntime js) : IVibration
{
    private bool isSupported;

    public async Task InitializeAsync()
    {
        isSupported = await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "vibrationIsSupported");
    }

    public bool IsSupported
    {
        get
        {
            BrowserEssentials.EnsureInitialized();
            return isSupported;
        }
    }

    public void Vibrate() => Vibrate(TimeSpan.FromMilliseconds(500));

    public async void Vibrate(TimeSpan duration)
    {
        BrowserEssentials.EnsureInitialized();
        if (!IsSupported)
            throw new FeatureNotSupportedException("The Vibration API is not available in this browser.");
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "vibrate", duration.TotalMilliseconds);        
    }

    public async void Cancel()
    {
        BrowserEssentials.EnsureInitialized();
        if (IsSupported)
            await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "vibrate", 0);
    }
}
