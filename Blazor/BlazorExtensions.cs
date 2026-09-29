using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public static class BlazorExtensions
{
    public static void UseBlazorEssentials(this IServiceCollection services)
    {
        var lifetime = ServiceLifetime.Singleton;

        services.TryAdd(ServiceDescriptor.Describe(typeof(IAccelerometer), provider => new BrowserAccelerometer(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppActions), provider => new BlazorAppActions(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppInfo), provider => new BlazorAppInfo(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBarometer), provider => new BlazorBarometer(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBattery), provider => new BrowserBattery(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBrowser), provider => new BlazorBrowser(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IClipboard), provider =>new BlazorClipboard(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ICompass), provider =>new BrowserCompass(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IConnectivity), provider =>new BlazorConnectivity(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceDisplay), provider => new BrowserDeviceDisplay(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceInfo), provider =>new BrowserDeviceInfo(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IEmail), provider => new BlazorEmail(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFilePicker), provider =>new BlazorFilePicker(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFileSystem), provider =>new BlazorFileSystem(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFlashlight), provider => new BlazorFlashlight(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeocoding), provider => new BlazorGeocoding(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeolocation), provider => new BrowserGeolocation(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGyroscope), provider => new BrowserGyroscope(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IHapticFeedback), provider =>new BlazorHapticFeedback(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ILauncher), provider => new BlazorLauncher(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMagnetometer), provider =>new BlazorMagnetometer(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMap), provider => new BlazorMap(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMediaPicker), provider => new BlazorMediaPicker(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IOrientationSensor), provider => new BrowserOrientationSensor(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPhoneDialer), provider => new BlazorPhoneDialer(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPreferences), provider =>new BlazorPreferences(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IScreenshot), provider => new BlazorScreenshot(), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISecureStorage), provider => new BlazorSecureStorage(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISemanticScreenReader), provider => new BlazorSemanticScreenReader(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IShare), provider => new BlazorShare(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISms), provider => new BlazorSms(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ITextToSpeech), provider => new BlazorTextToSpeech(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVibration), provider => new BlazorVibration(GetRequiredProvider(provider).GetRequiredService<IJSRuntime>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVersionTracking), provider => new BlazorVersionTracking(GetRequiredProvider(provider).GetRequiredService<IPreferences>(), provider.GetRequiredService<IAppInfo>()), lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IWebAuthenticator), provider => WebAuthenticator.Default, lifetime));

        IServiceProvider GetRequiredProvider(IServiceProvider provider)
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            return provider;
        }
    }
}
