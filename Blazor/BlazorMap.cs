using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

/// <summary>Map apps cannot be launched from the browser backend.</summary>
public class BlazorMap : IMap
{
    public Task OpenAsync(double latitude, double longitude, MapLaunchOptions options) =>
        throw new FeatureNotSupportedException("Opening a maps app is not supported in the browser.");

    public Task OpenAsync(Placemark placemark, MapLaunchOptions options) =>
        throw new FeatureNotSupportedException("Opening a maps app is not supported in the browser.");

    public Task<bool> TryOpenAsync(double latitude, double longitude, MapLaunchOptions options) =>
        Task.FromResult(false);

    public Task<bool> TryOpenAsync(Placemark placemark, MapLaunchOptions options) =>
        Task.FromResult(false);
}
