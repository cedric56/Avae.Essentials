using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public static class BlazorEssentials
{
    private static string? _moduleUrl = string.Empty;

    public static bool IsInitialized =>
        Module != null;

    /// <summary>
    /// Throws when the module has not been imported yet. Used by synchronous API surfaces
    /// that cannot await initialization themselves.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException(
                "Blazor Essentials is not initialized. Call " +
                "'await BlazorEssentials.InitializeAsync(js, moduleurl)' during app startup " +
                "before using Essentials APIs.");
    }

    public static async Task InitializeAsync(
       IServiceProvider provider,        
       string moduleUrl)
    {
        _moduleUrl = moduleUrl;

        var js = provider.GetRequiredService<IJSRuntime>();

        Module = await InvokeCoreAsync(js);

        var connectivity = (BlazorConnectivity)provider.GetRequiredService<IConnectivity>();
        await connectivity.InitializeAsync();

        var appInfo = (BlazorAppInfo)provider.GetRequiredService<IAppInfo>();
        await appInfo.InitializeAsync();

        var feedback = (BlazorHapticFeedback)provider.GetRequiredService<IHapticFeedback>();
        await feedback.InitializeAsync();

        var preferences = (BlazorPreferences)provider.GetRequiredService<IPreferences>();
        await preferences.InitializeAsync();

        var storage = (BlazorSecureStorage)provider.GetRequiredService<ISecureStorage>();
        await storage.InitializeAsync();

        var battery = (BlazorBattery)provider.GetRequiredService<IBattery>();
        await battery.InitializeAsync();

        var info = (BlazorDeviceInfo)provider.GetRequiredService<IDeviceInfo>();
        await info.InitializeAsync();

        var vibration = (BlazorVibration)provider.GetRequiredService<IVibration>();
        await vibration.InitializeAsync();

        var display = (BlazorDeviceDisplay)provider.GetRequiredService<IDeviceDisplay>();
        await display.InitializeAsync();

        var accelerometer = (BlazorAccelerometer)provider.GetRequiredService<IAccelerometer>();
        await accelerometer.InitializeAsync();

        var gyroscope = (BlazorGyroscope)provider.GetRequiredService<IGyroscope>();
        await gyroscope.InitializeAsync();

        var compass = (BlazorCompass)provider.GetRequiredService<ICompass>();
        await compass.InitializeAsync();

        var orientation = (BlazorOrientationSensor)provider.GetRequiredService<IOrientationSensor>();
        await orientation.InitializeAsync();

        var share = (BlazorShare)provider.GetRequiredService<IShare>();
        await share.InitializeAsync();
    }

    internal static async Task<IJSObjectReference> InvokeCoreAsync(IJSRuntime js)
    {
        if (js == null)
            throw new InvalidOperationException();

        return await js.InvokeAsync<IJSObjectReference>("import", _moduleUrl)
                       .ConfigureAwait(false);
    }

   internal  static IJSObjectReference Module { get; set; } = null!;
}
