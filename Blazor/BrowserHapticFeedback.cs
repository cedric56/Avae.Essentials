using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Haptic feedback approximated with short navigator.vibrate pulses.</summary>
public class BrowserHapticFeedback(IJSRuntime js) : IHapticFeedback
{
    public bool IsSupported
	{
		get
		{
			BrowserEssentials.EnsureInitialized();
			return BrowserEssentials.VibrationIsSupported(js);
		}
	}

	public async void Perform(HapticFeedbackType type = HapticFeedbackType.Click)
	{
		BrowserEssentials.EnsureInitialized();
		if (!IsSupported)
			throw new FeatureNotSupportedException("The Vibration API is not available in this browser.");
		await BrowserEssentials.VibrateAsync(js, type == HapticFeedbackType.LongPress ? 25 : 10);
	}
}
