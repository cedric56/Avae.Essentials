using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace Avae.Essentials;

/// <summary>Haptic feedback approximated with short navigator.vibrate pulses.</summary>
public class BlazorHapticFeedback(IJSRuntime js) : IHapticFeedback
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
            BlazorEssentials.EnsureInitialized();
            return isSupported;
        }
    }

    public async void Perform(HapticFeedbackType type = HapticFeedbackType.Click)
    {
        if (!IsSupported)
            throw new FeatureNotSupportedException("The Vibration API is not available in this browser.");
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "vibrate", type == HapticFeedbackType.LongPress ? 25 : 10);
    }
}
