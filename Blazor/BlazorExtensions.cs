using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Authentication;

namespace Avae.Essentials;

public static class BlazorExtensions
{
    public static void UseBlazorEssentials(this IServiceCollection services)
    {
        var appInfo = new BlazorAppInfo();
        var preferences = new BlazorPreferences();
        services.SetDefaults(
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
            () =>new BlazorSecureStorage(),
            new BlazorSemanticScreenReader(),
            new BlazorShare(),
            new BlazorSms(),
            new BlazorTextToSpeech(),
            new BlazorVibration(),
            WebAuthenticator.Default,
            () => new BlazorVersionTracking(preferences, appInfo),
            ServiceLifetime.Singleton);
    }
}
