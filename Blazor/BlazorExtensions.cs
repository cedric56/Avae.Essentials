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
using System;
using static Avae.Essentials.Extensions;

namespace Avae.Essentials;

internal static class BlazorExtensions
{
    public static void UseBlazorEssentials(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {        
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAccelerometer), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var accelerometer = new BrowserAccelerometer(provider.GetRequiredService<IJSRuntime>());
            return accelerometer;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppActions), provider =>
        {
            var appActions = new BrowserAppActions();
            return appActions;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppInfo), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var appInfo = new BrowserAppInfo();
            _ = appInfo.InitializeAsync(provider.GetRequiredService<IJSRuntime>());
            return appInfo;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IBarometer), provider =>
        {
            var barometer = new BrowserBarometer();
            return barometer;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IBattery), provider =>
        {
            var battery = new BrowserBattery();
            return battery;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IBrowser), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var browser = new BrowserBrowser(provider.GetRequiredService<IJSRuntime>());
            return browser;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IClipboard), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var clipboard = new BrowserClipboard(provider.GetRequiredService<IJSRuntime>());
            return clipboard;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ICompass), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var compass = new BrowserCompass(provider.GetRequiredService<IJSRuntime>());
            return compass;
        }, lifetime));

        //services.TryAdd(ServiceDescriptor.Describe(typeof(IConnectivity), provider =>
        //{
        //    var injected = CircuitServiceAccessor.Provider;
        //    if (injected != null)
        //        provider = injected;
        //    var connectivity = new BlazorConnectivity(provider.GetRequiredService<IJSRuntime>());
        //    return connectivity;
        //}, lifetime));

        services.AddBrowserConnectivity(lifetime);

        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceDisplay), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var display = new BrowserDeviceDisplay(provider.GetRequiredService<IJSRuntime>());
            return display;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceInfo), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var deviceInfo = new BrowserDeviceInfo(provider.GetRequiredService<IJSRuntime>());
            return deviceInfo;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IEmail), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var email = new BrowserEmail(provider.GetRequiredService<IJSRuntime>());
            return email;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IFilePicker), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var filePicker = new BrowserFilePicker(provider.GetRequiredService<IJSRuntime>());
            return filePicker;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IFileSystem), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var fileSystem = new BrowserFileSystem(provider.GetRequiredService<IJSRuntime>());
            return fileSystem;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IFlashlight), provider =>
        {
            var flashlight = new BrowserFlashlight();
            return flashlight;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeocoding), provider =>
        {
            var geocoding = new BrowserGeocoding();
            return geocoding;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeolocation), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var geolocation = new BrowserGeolocation(provider.GetRequiredService<IJSRuntime>());
            return geolocation;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IGyroscope), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var gyroscope = new BrowserGyroscope(provider.GetRequiredService<IJSRuntime>());
            return gyroscope;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IHapticFeedback), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var haptic = new BrowserHapticFeedback(provider.GetRequiredService<IJSRuntime>());
            return haptic;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ILauncher), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var launcher = new BrowserLauncher(provider.GetRequiredService<IJSRuntime>());
            return launcher;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IMagnetometer), provider =>
        {
            var magnetometer = new BrowserMagnetometer();
            return magnetometer;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IMap), provider =>
        {
            var map = new BrowserMap();
            return map;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IMediaPicker), provider =>
        {
            var mediaPicker = new BrowserMediaPicker();
            return mediaPicker;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IOrientationSensor), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var orientation = new BrowserOrientationSensor(provider.GetRequiredService<IJSRuntime>());
            return orientation;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IPhoneDialer), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var dialer = new BrowserPhoneDialer(provider.GetRequiredService<IJSRuntime>());
            return dialer;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IPreferences), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var prefs = new BrowserPreferences(provider.GetRequiredService<IJSRuntime>());
            return prefs;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IScreenshot), provider =>
        {
            var screenshot = new BrowserScreenshot();
            return screenshot;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ISecureStorage), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var storage = new BrowserSecureStorage(provider.GetRequiredService<IJSRuntime>());
            return storage;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ISemanticScreenReader), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var reader = new BrowserSemanticScreenReader(provider.GetRequiredService<IJSRuntime>());
            return reader;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IShare), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var share = new BrowserShare(provider.GetRequiredService<IJSRuntime>());
            return share;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ISms), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var sms = new BrowserSms(provider.GetRequiredService<IJSRuntime>());
            return sms;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(ITextToSpeech), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var tts = new BrowserTextToSpeech(provider.GetRequiredService<IJSRuntime>());
            return tts;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IVibration), provider =>
        {
            var injected = CircuitServiceAccessor.Provider;
            if (injected != null)
                provider = injected;
            var vibration = new BrowserVibration(provider.GetRequiredService<IJSRuntime>());
            return vibration;
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IVersionTracking), provider =>
        {
            try
            {
                var injected = CircuitServiceAccessor.Provider;
                if (injected != null)
                    provider = injected;
                var tracking = new BrowserVersionTracking(provider.GetRequiredService<IPreferences>(), provider.GetRequiredService<IAppInfo>());
                return tracking;
            }
            catch
            {
                return new VersionTrackingDefault();
            }
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IWebAuthenticator), provider =>
        {
            //var auth = new BrowserWebAuthenticator();
            //_ = auth.InitializeAsync(provider.GetRequiredService<IJSRuntime>());
            //return auth;
            return WebAuthenticator.Default;
        }, lifetime));
    }

    private static IServiceCollection AddBrowserConnectivity(this IServiceCollection services, ServiceLifetime lifetime)
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(BlazorConnectivity), provider =>
        {
                var injected = CircuitServiceAccessor.Provider;
                if (injected != null)
                    provider = injected;
                var connectivity = new BlazorConnectivity(provider.GetRequiredService<IJSRuntime>());
                return connectivity;            
        }, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IConnectivity),
            sp => sp.GetRequiredService<BlazorConnectivity>(), lifetime));
        return services;
    }
}
