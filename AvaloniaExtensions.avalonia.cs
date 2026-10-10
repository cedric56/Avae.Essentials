using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;
using System;
using System.Collections.Generic;
using AvaloniaFileSystem = Avalonia.Controls.Maui.Essentials.AvaloniaFileSystem;
using AvaloniaHapticFeedback = Avalonia.Controls.Maui.Essentials.AvaloniaHapticFeedback;
using AvaloniaPreferences = Avalonia.Controls.Maui.Essentials.AvaloniaPreferences;
using AvaloniaScreenshot = Avalonia.Controls.Maui.Essentials.AvaloniaScreenshot;
using AvaloniaMediaPicker = Avalonia.Controls.Maui.Essentials.AvaloniaMediaPicker;
using Dispatcher = Avalonia.Threading.Dispatcher;

namespace Avae.Essentials;

public static class AvaloniaExtensions
{
    public static void UseAvaloniaEssentials(this IServiceCollection services)
    {
        AvaeDispatcher.Initialize(Dispatcher.CurrentDispatcher);

        var platformProvider = new AvaeTopLevelStateManager();
        var screenshot = new AvaloniaScreenshot(platformProvider);
        var filepicker = (Microsoft.Maui.Storage.IFilePicker)AvaloniaDefaults.CreateAvaloniaFilePicker(platformProvider);
        var mediapicker = (Microsoft.Maui.Media.IMediaPicker)AvaloniaDefaults.CreateAvaloniaMediaPicker(platformProvider);
        var hapticFeedback = new AvaloniaHapticFeedback();
        var preferences = new AvaloniaPreferences();
        var fileSystem = new AvaloniaFileSystem();
        var webAuthenticator = (Microsoft.Maui.Authentication.IWebAuthenticator)AvaloniaDefaults.CreateAvaloniaWebAuthenticator(platformProvider);

        if (OperatingSystem.IsWindows())
        {
            services.SetDefaults(
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AccelerometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10586, 0) ? new AppActionsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeAppInfo() : new AppInfoDefault(),
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BarometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BatteryImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BrowserImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ClipboardImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new CompassImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ConnectivityImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ContactsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeDeviceDisplay() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new DeviceInfoImplementation() : null!,
            new Avae.Essentials.AvaeEmail(),
            filepicker,
            fileSystem,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new FlashlightImplementation() : null!,
#if WINDOWS
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763, 0) ? Microsoft.Maui.Devices.Sensors.Geocoding.Default : null!,
#else
            new AvaeGeocoding(),
#endif
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393, 0) ? new GeolocationImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new GyroscopeImplementation() : null!,
            hapticFeedback,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new LauncherImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new MagnetometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new MapImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeMediaPicker((AvaloniaMediaPicker)mediapicker) : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.OrientationSensorImplementation() : null!,
            new AvaePhoneDialer(),
            preferences,
            screenshot,
            () => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new SecureStorageImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeSemanticScreenReader() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ShareImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new SmsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new TextToSpeechImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new VibrationImplementation() : null!,
            webAuthenticator,
            () => Microsoft.Maui.ApplicationModel.VersionTracking.Default);
        }
        else if (OperatingSystem.IsLinux())
        {
            services.SetDefaults(
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxAccelerometer(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.AppModel.LinuxAppActions(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.AppModel.LinuxAppInfo(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxBarometer(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Devices.LinuxBattery(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.AppModel.LinuxBrowser(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.DataTransfer.LinuxClipboard(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxCompass(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Networking.LinuxConnectivity(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Communication.LinuxContacts(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Devices.LinuxDeviceDisplay(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Devices.LinuxDeviceInfo(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Communication.LinuxEmail(),
                filepicker,
                fileSystem,
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Devices.LinuxFlashlight(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxGeocoding(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxGeolocation(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxGyroscope(),
                hapticFeedback,
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.AppModel.LinuxLauncher(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxMagnetometer(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.AppModel.LinuxMap(),
                mediapicker,
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Sensors.LinuxOrientationSensor(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Communication.LinuxPhoneDialer(),
                preferences,
                screenshot,
                () => OperatingSystem.IsLinux() ? new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Storage.LinuxSecureStorage() : null!,
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Accessibility.LinuxSemanticScreenReader(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.DataTransfer.LinuxShare(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Communication.LinuxSms(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Media.LinuxTextToSpeech(),
                new Microsoft.Maui.Platforms.Linux.Gtk4.Essentials.Devices.LinuxVibration(),
                webAuthenticator,
                () => VersionTracking.Default);
        }
        else if (OperatingSystem.IsMacOS())
        {
#if MACOS
            services.SetDefaults(
                null!,
                null!,
                (Microsoft.Maui.ApplicationModel.IAppInfo)MacosDefaults.CreateAppInfo(),
                null!,
                (Microsoft.Maui.Devices.IBattery)MacosDefaults.CreateBattery(),
                (Microsoft.Maui.ApplicationModel.IBrowser)MacosDefaults.CreateBrowser(),
                (Microsoft.Maui.ApplicationModel.DataTransfer.IClipboard)MacosDefaults.CreateClipboard(),
                null!,
                (Microsoft.Maui.Networking.IConnectivity)MacosDefaults.CreateConnectivity(),
                null!,
                (Microsoft.Maui.Devices.IDeviceDisplay)MacosDefaults.CreateDeviceDisplay(),
                (Microsoft.Maui.Devices.IDeviceInfo)MacosDefaults.CreateDeviceInfo(),
                (Microsoft.Maui.ApplicationModel.Communication.IEmail)MacosDefaults.CreateEmail(),
                filepicker,
                fileSystem,
                (Microsoft.Maui.Devices.IFlashlight)MacosDefaults.CreateFlashlight(),
                null!,
                (Microsoft.Maui.Devices.Sensors.IGeolocation)MacosDefaults.CreateGeolocation(),
                null!,
                hapticFeedback,
                (Microsoft.Maui.ApplicationModel.ILauncher)MacosDefaults.CreateLauncher(),
                null!,
                (Microsoft.Maui.ApplicationModel.IMap)MacosDefaults.CreateMap(),
                mediapicker,
                null!,
                (Microsoft.Maui.ApplicationModel.Communication.IPhoneDialer)MacosDefaults.CreatePhoneDialer(),
                preferences,
                screenshot,
                () => (Microsoft.Maui.Storage.ISecureStorage)MacosDefaults.CreateSecureStorage(),
                (Microsoft.Maui.Accessibility.ISemanticScreenReader)MacosDefaults.CreateSemanticScreenReader(),
                (Microsoft.Maui.ApplicationModel.DataTransfer.IShare)MacosDefaults.CreateShare(),
                (Microsoft.Maui.ApplicationModel.Communication.ISms)MacosDefaults.CreateSms(),
                (Microsoft.Maui.Media.ITextToSpeech)MacosDefaults.CreateTextToSpeech(),
                (Microsoft.Maui.Devices.IVibration)MacosDefaults.CreateVibration(),
                webAuthenticator,
                () => Microsoft.Maui.ApplicationModel.VersionTracking.Default);
#endif
        }
        else if (OperatingSystem.IsBrowser())
        {
            services.SetDefaults(
                null!,
                null!,
                new AppInfoDefault(),
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                filepicker,
                fileSystem,
                null!,
                null!,
                null!,
                null!,
                hapticFeedback,
                null!,
                null!,
                null!,
                mediapicker,
                null!,
                null!,
                preferences,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                webAuthenticator,
                () => Microsoft.Maui.ApplicationModel.VersionTracking.Default);
        }
        else
        {
            services.SetDefaults(
                null!,
                null!,
                new AppInfoDefault(),
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                filepicker,
                fileSystem,
                null!,
                null!,
                null!,
                null!,
                hapticFeedback,
                null!,
                null!,
                null!,
                mediapicker,
                null!,
                null!,
                preferences,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                webAuthenticator,
                () => Microsoft.Maui.ApplicationModel.VersionTracking.Default);
        }

        services.TryAdd(ServiceDescriptor.Describe(typeof(IDispatcher), _ => AvaeDispatcher.Default, ServiceLifetime.Singleton));
        services.RegisterEssentials();
    }

    class AppInfoDefault : IAppInfo
    {
        public string PackageName => string.Empty;

        public string Name => string.Empty;

        public string VersionString => string.Empty;

        public Version Version => null!;

        public string BuildString => string.Empty;

        public AppTheme RequestedTheme => AppTheme.Unspecified;

        public AppPackagingModel PackagingModel => AppPackagingModel.Unpackaged;

        public LayoutDirection RequestedLayoutDirection => LayoutDirection.Unknown;

        public void ShowSettingsUI()
        {
            throw new NotImplementedException();
        }
    }
}
