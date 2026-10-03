using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace Avae.Essentials;

public static class BlazorEssentials
{
    public static void UseBlazorEssentials(this IServiceCollection services)
    {
        var appInfo = new BlazorAppInfo();
        var preferences = new BlazorPreferences();
        services.SetDefaultsAndRegister(
            new BlazorAccelerometer(),
            new BlazorAppActions(),
            appInfo,
            new BlazorBarometer(),
            new BlazorBattery(),
            new BlazorBrowser(),
            new BlazorClipboard(),
            new BlazorCompass(),
            new BlazorConnectivity(),
            new BlazorContacts(),
            new BlazorDeviceDisplay(),
            new BlazorDeviceInfo(),
            new BlazorEmail(),
            new BlazorFilePicker(),
            new BlazorFileSystem(),
            new BlazorFlashlight(),
            new BlazorGeocoding(),
            new BlazorGeolocation(),
            new BlazorGyroscope(),
            new BlazorHapticFeedback(),
            new BlazorLauncher(),
            new BlazorMagnetometer(),
            new BlazorMap(),
            new BlazorMediaPicker(),
            new BlazorOrientationSensor(),
            new BlazorPhoneDialer(),
            preferences,
            new BlazorScreenshot(),
            () => new BlazorSecureStorage(),
            new BlazorSemanticScreenReader(),
            new BlazorShare(),
            new BlazorSms(),
            new BlazorTextToSpeech(),
            new BlazorVibration(),
            WebAuthenticator.Default,
            () => new BlazorVersionTracking(preferences, appInfo),
            ServiceLifetime.Singleton);
    }

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

    public static async Task InitializeAsync(IServiceProvider provider)
    {
        var js = provider.GetRequiredService<IJSRuntime>();

        Module?.DisposeAsync();
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

        var mediaPicker = (BlazorMediaPicker)provider.GetRequiredService<IMediaPicker>();
        await mediaPicker.InitializeAsync();
    }

    internal static async Task<IJSObjectReference> InvokeCoreAsync(IJSRuntime js)
    {
        if (js == null)
            throw new InvalidOperationException();

        return await js.InvokeAsync<IJSObjectReference>("import", "./BlazorEssentials.js")
                       .ConfigureAwait(false);
    }

   internal  static IJSObjectReference Module { get; set; } = null!;
}
