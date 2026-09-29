using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace Avae.Essentials;

/// <summary>The camera torch is not reachable from the browser.</summary>
public class BlazorFlashlight : IFlashlight
{
    public Task<bool> IsSupportedAsync() => Task.FromResult(false);

    public Task TurnOnAsync() =>
        throw new FeatureNotSupportedException("The flashlight is not available in the browser.");

    public Task TurnOffAsync() =>
        throw new FeatureNotSupportedException("The flashlight is not available in the browser.");
}
