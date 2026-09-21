using Microsoft.Extensions.DependencyInjection;
using Append.Blazor.WebShare;
using BlazorNative.Core;
using BlazorNative.Device;
using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.Blazor.MediaCaptureStreams;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using PatrickJahr.Blazor.AsyncClipboard;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Avae.Essentials;

public static class BlazorExtensions
{
    public static void UseBlazorEssentials(this IServiceCollection services)
    {
        var circuit = new CircuitServiceAccessor();
        services.AddSingleton<IMobileBridge, DevHostBridge>();
        services.AddFileSystemAccessService();
        services.AddWebShare();
        services.AddAsyncClipboardService();
        services.AddMediaDevicesService();
        services.AddSpeechSynthesis();
        services.AddSingleton<IMobileBridge, DevHostBridge>();
        services.AddBlazorNativeDevice();
        services.AddSingleton<VideoCaptureCoordinator>();
        services.TryAddScoped<BlazorSensors.Accelerometer>();
        services.TryAddScoped<BlazorSensors.Gyroscope>();
        services.TryAddScoped<BlazorSensors.Magnetometer>();
        services.TryAddScoped<BlazorSensors.AbsoluteOrientationSensor>();

        services.SetDefaults(
          new BlazorAccelerometer(circuit),
          AppActions.Current,
          AppInfo.Current,
          Barometer.Default,
          Battery.Default,
          new BlazorBrowser(circuit),
          new BlazorClipboard(circuit),
          Compass.Default,
          Connectivity.Current,
          Contacts.Default,
          DeviceDisplay.Current,
          DeviceInfo.Current,
          Email.Default,
          new BlazorFilePicker(circuit),
          FileSystem.Current,
          Flashlight.Default,
          new BlazorGeocoding(circuit),
          Geolocation.Default,
          new BlazorGyroscope(circuit),
          HapticFeedback.Default,
          new BlazorLauncher(circuit),
          new BlazorMagnetometer(circuit),
          Map.Default,
          new BlazorMediaPicker(circuit),
          new BlazorOrientationSensor(circuit),
          new BlazorPhoneDialer(circuit),
          Preferences.Default,
          Screenshot.Default,
          () => new BlazorSecureStorage(circuit),
          SemanticScreenReader.Default,
          new BlazorShare(circuit),
          new BlazorSms(circuit),
          new BlazorTextToSpeech(circuit),
          Vibration.Default,
          WebAuthenticator.Default,
          null!,
          //VersionTracking.Default,
          ServiceLifetime.Scoped);
    }
}
