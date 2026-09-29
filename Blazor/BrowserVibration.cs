using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Vibration backed by navigator.vibrate (mobile browsers; no-ops when unsupported by hardware).</summary>
public class BrowserVibration(IJSRuntime js) : IVibration
{
    public bool IsSupported
	{
		get
		{
			BrowserEssentials.EnsureInitialized();
			return BrowserEssentials.VibrationIsSupported(js);
		}
	}

	public void Vibrate() => Vibrate(TimeSpan.FromMilliseconds(500));

	public void Vibrate(TimeSpan duration)
	{
		BrowserEssentials.EnsureInitialized();
		if (!IsSupported)
			throw new FeatureNotSupportedException("The Vibration API is not available in this browser.");
		_ = BrowserEssentials.VibrateAsync(js, duration.TotalMilliseconds);
	}

	public void Cancel()
	{
		BrowserEssentials.EnsureInitialized();
		if (IsSupported)
			_ = BrowserEssentials.VibrateAsync(js, 0);
	}
}
