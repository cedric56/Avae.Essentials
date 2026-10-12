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
using Avalonia;

namespace Avae.Essentials;

public static class AvaloniaExtensions
{
    /// <summary>
    /// Might use method on directly via services
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAvaeEssentials(this AppBuilder builder)
    {
        var services = new ServiceCollection();
        services.UseAvaeEssentials();
        return builder;
    }

    public static void UseAvaeEssentials(this IServiceCollection services)
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
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalAccelerometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10586, 0) ? new OriginalAppActionsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeAppInfo() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalBarometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalBatteryImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalBrowserImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalClipboardImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalCompassImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalConnectivityImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalContactsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeDeviceDisplay() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalDeviceInfoImplementation() : null!,
            new Avae.Essentials.AvaeEmail(),
            filepicker,
            fileSystem,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalFlashlightImplementation() : null!,
#if WINDOWS
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763, 0) ? Microsoft.Maui.Devices.Sensors.Geocoding.Default : null!,
#else
            new AvaeGeocoding(),
#endif
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393, 0) ? new AvaeGeolocation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalGyroscopeImplementation() : null!,
            hapticFeedback,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalLauncherImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalMagnetometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalMapImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeMediaPicker((AvaloniaMediaPicker)mediapicker) : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalOrientationSensorImplementation() : null!,
            new AvaePhoneDialer(),
            preferences,
            screenshot,
            () => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalSecureStorageImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeSemanticScreenReader() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalShareImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalSmsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalTextToSpeechImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OriginalVibrationImplementation() : null!,
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
            NSApplication.CheckForIllegalCrossThreadCalls = false;

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
        services.RegisterAvaeEssentials();
    }
}
