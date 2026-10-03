using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using System;
using System.Threading.Tasks;

namespace Avae.Essentials;

/// <summary>Vibration backed by navigator.vibrate (mobile browsers; no-ops when unsupported by hardware).</summary>
public class BlazorVibration : IVibration
{
    private bool isSupported;

    public async Task InitializeAsync()
    {
        isSupported = await BlazorEssentials.Module.InvokeAsync<bool>("vibrationIsSupported");
    }

    public bool IsSupported
    {
        get
        {
            BlazorEssentials.EnsureInitialized();
            return isSupported;
        }
    }

    public void Vibrate() => Vibrate(TimeSpan.FromMilliseconds(500));

    public async void Vibrate(TimeSpan duration)
    {
        BlazorEssentials.EnsureInitialized();
        if (!IsSupported)
            throw new FeatureNotSupportedException("The Vibration API is not available in this browser.");
        await BlazorEssentials.Module.InvokeVoidAsync("vibrate", duration.TotalMilliseconds);        
    }

    public async void Cancel()
    {
        BlazorEssentials.EnsureInitialized();
        if (IsSupported)
            await BlazorEssentials.Module.InvokeVoidAsync("vibrate", 0);
    }
}
