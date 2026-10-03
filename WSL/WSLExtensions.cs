using Avalonia.Controls.Maui.Essentials;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Runtime.Versioning;

namespace Avae.Essentials;

public static class WSLExtensions
{
    [SupportedOSPlatform("linux")]
    public static void OnWslEssentials(this IServiceCollection services)
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

        services.SetDefaultsAndRegister(
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
}
