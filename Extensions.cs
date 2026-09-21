using Avalonia.Controls.Maui.Essentials;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using static Avae.Essentials.Extensions;

namespace Avae.Essentials;

/// <summary>
/// Extension methods for registering .NET MAUI Essentials services with dependency injection,
/// looking up locale-to-country mappings, and bridging Avae-specific file/email/share behavior
/// with the underlying MAUI Essentials APIs.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Assigns the supplied MAUI Essentials implementations as the current defaults (via
    /// <see cref="EssentialsAccessors"/>), then registers them with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="accelerometer">The accelerometer implementation to use as the default.</param>
    /// <param name="appActions">The app actions implementation to use as the default.</param>
    /// <param name="appInfo">The app info implementation to use as the default.</param>
    /// <param name="barometer">The barometer implementation to use as the default.</param>
    /// <param name="battery">The battery implementation to use as the default.</param>
    /// <param name="browser">The browser implementation to use as the default.</param>
    /// <param name="clipboard">The clipboard implementation to use as the default.</param>
    /// <param name="compass">The compass implementation to use as the default.</param>
    /// <param name="connectivity">The connectivity implementation to use as the default.</param>
    /// <param name="contacts">The contacts implementation to use as the default.</param>
    /// <param name="deviceDisplay">The device display implementation to use as the default.</param>
    /// <param name="deviceInfo">The device info implementation to use as the default.</param>
    /// <param name="email">The email implementation to use as the default.</param>
    /// <param name="filepicker">The file picker implementation to use as the default.</param>
    /// <param name="fileSystem">The file system implementation to use as the default.</param>
    /// <param name="flashlight">The flashlight implementation to use as the default.</param>
    /// <param name="geocoding">The geocoding implementation to use as the default.</param>
    /// <param name="geolocation">The geolocation implementation to use as the default.</param>
    /// <param name="gyroscope">The gyroscope implementation to use as the default.</param>
    /// <param name="hapticFeedback">The haptic feedback implementation to use as the default.</param>
    /// <param name="launcher">The launcher implementation to use as the default.</param>
    /// <param name="magnetometer">The magnetometer implementation to use as the default.</param>
    /// <param name="map">The map implementation to use as the default.</param>
    /// <param name="mediaPicker">The media picker implementation to use as the default.</param>
    /// <param name="orientationSensor">The orientation sensor implementation to use as the default.</param>
    /// <param name="phoneDialer">The phone dialer implementation to use as the default.</param>
    /// <param name="preferences">The preferences implementation to use as the default.</param>
    /// <param name="screenshot">The screenshot implementation to use as the default.</param>
    /// <param name="secureStorage">A factory returning the secure storage implementation to use as the default.</param>
    /// <param name="semanticScreenReader">The semantic screen reader implementation to use as the default.</param>
    /// <param name="share">The share implementation to use as the default.</param>
    /// <param name="sms">The SMS implementation to use as the default.</param>
    /// <param name="textToSpeech">The text-to-speech implementation to use as the default.</param>
    /// <param name="vibration">The vibration implementation to use as the default.</param>
    /// <param name="webAuthenticator">The web authenticator implementation to use as the default.</param>
    /// <param name="versionTracking">A factory returning the version tracking implementation to use as the default.</param>
    /// <param name="lifetime">The service lifetime used when registering these implementations with the container. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
    public static void SetDefaults(this IServiceCollection services,
        IAccelerometer accelerometer,
        IAppActions appActions,
        IAppInfo appInfo,
        IBarometer barometer,
        IBattery battery,
        IBrowser browser,
        IClipboard clipboard,
        ICompass compass,
        IConnectivity connectivity,
        IContacts contacts,
        IDeviceDisplay deviceDisplay,
        IDeviceInfo deviceInfo,
        IEmail email,
        IFilePicker filepicker,
        IFileSystem fileSystem,
        IFlashlight flashlight,
        IGeocoding geocoding,
        IGeolocation geolocation,
        IGyroscope gyroscope,
        IHapticFeedback hapticFeedback,
        ILauncher launcher,
        IMagnetometer magnetometer,
        IMap map,
        IMediaPicker mediaPicker,
        IOrientationSensor orientationSensor,
        IPhoneDialer phoneDialer,
        IPreferences preferences,
        IScreenshot screenshot,
        Func<ISecureStorage> secureStorage,
        ISemanticScreenReader semanticScreenReader,
        IShare share,
        ISms sms,
        ITextToSpeech textToSpeech,
        IVibration vibration,
        IWebAuthenticator webAuthenticator,
        Func<IVersionTracking> versionTracking,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        EssentialsAccessors.SetScreenshot(null, screenshot);
        EssentialsAccessors.SetFilePicker(null, filepicker);
        EssentialsAccessors.SetMediaPicker(null, mediaPicker);
        EssentialsAccessors.SetHapticFeedback(null, hapticFeedback);
        EssentialsAccessors.SetPreferences(null, preferences);
        EssentialsAccessors.SetFileSystem(null, fileSystem);
        EssentialsAccessors.SetWebAuthenticator(null, webAuthenticator);
        EssentialsAccessors.SetAccelerometer(null, accelerometer);
        EssentialsAccessors.SetAppActions(null, appActions);
        EssentialsAccessors.SetAppInfo(null, appInfo);
        EssentialsAccessors.SetBarometer(null, barometer);
        EssentialsAccessors.SetBattery(null, battery);
        EssentialsAccessors.SetBrowser(null, browser);
        EssentialsAccessors.SetClipboard(null, clipboard);
        EssentialsAccessors.SetCompass(null, compass);
        EssentialsAccessors.SetConnectivity(null, connectivity);
        EssentialsAccessors.SetContacts(null, contacts);
        EssentialsAccessors.SetDeviceDisplay(null, deviceDisplay);
        EssentialsAccessors.SetDeviceInfo(null, deviceInfo);
        EssentialsAccessors.SetEmail(null, email);
        EssentialsAccessors.SetFlashlight(null, flashlight);
        EssentialsAccessors.SetGeocoding(null, geocoding);
        EssentialsAccessors.SetGeocolation(null, geolocation);
        EssentialsAccessors.SetGyroscope(null, gyroscope);
        EssentialsAccessors.SetLauncher(null, launcher);
        EssentialsAccessors.SetMagnetometer(null, magnetometer);
        EssentialsAccessors.SetMap(null, map);
        EssentialsAccessors.SetOrientationSensor(null, orientationSensor);
        EssentialsAccessors.SetPhoneDialer(null, phoneDialer);
        EssentialsAccessors.SetSecureStorage(null, secureStorage?.Invoke());
        EssentialsAccessors.SetSemanticScreenReader(null, semanticScreenReader);
        EssentialsAccessors.SetShare(null, share);
        EssentialsAccessors.SetSms(null, sms);
        EssentialsAccessors.SetTextToSpeech(null, textToSpeech);
        EssentialsAccessors.SetVibration(null, vibration);
        EssentialsAccessors.SetVersionTracking(null, versionTracking?.Invoke());

        services.RegisterEssentials(lifetime);
    }

    /// <summary>
    /// Registers the platform-default implementations of all MAUI Essentials service interfaces
    /// with the dependency injection container, without overwriting any already-registered implementation.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="lifetime">The service lifetime used when registering these implementations. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
    public static void RegisterEssentials(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDispatcher), _ => AvaeDispatcher.Default, lifetime));

        services.TryAdd(ServiceDescriptor.Describe(typeof(IAccelerometer), _ => Accelerometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppActions), _ => AppActions.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppInfo), _ => AppInfo.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBarometer), _ => Barometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBattery), _ => Battery.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBrowser), _ => Browser.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IClipboard), _ => Clipboard.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ICompass), _ => Compass.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IConnectivity), _ => Connectivity.Current, lifetime));
        //services.TryAdd(ServiceDescriptor.Describe(typeof(IContacts), _ =>  Contacts.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceDisplay), _ => DeviceDisplay.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceInfo), _ => DeviceInfo.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IEmail), _ => Email.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFilePicker), _ => FilePicker.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFileSystem), _ => FileSystem.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFlashlight), _ => Flashlight.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeocoding), _ => Geocoding.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeolocation), _ => Geolocation.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGyroscope), _ => Gyroscope.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IHapticFeedback), _ => HapticFeedback.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ILauncher), _ => Launcher.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMagnetometer), _ => Magnetometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMap), _ => Map.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMediaPicker), _ => MediaPicker.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IOrientationSensor), _ => OrientationSensor.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPhoneDialer), _ => PhoneDialer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPreferences), _ => Preferences.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IScreenshot), _ => Screenshot.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISecureStorage), _ => SecureStorage.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISemanticScreenReader), _ => SemanticScreenReader.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IShare), _ => Share.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISms), _ => Sms.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ITextToSpeech), _ => TextToSpeech.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVibration), _ => Vibration.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVersionTracking), _ =>
        {
            try
            {
                return VersionTracking.Default;
            }
            catch
            {
                return new VersionTrackingDefault();
            }

        }, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IWebAuthenticator), _ => WebAuthenticator.Default, lifetime));
    }

    public static void UseEssentials(this IServiceCollection services)
    {
        AvaeDispatcher.Initialize(Avalonia.Threading.Dispatcher.CurrentDispatcher);

        var platformProvider = new AvaeTopLevelStateManager();
        var screenshot = new AvaloniaScreenshot(platformProvider);
        var filepicker = (Microsoft.Maui.Storage.IFilePicker)AvaloniaDefaults.CreateAvaloniaFilePicker(platformProvider);
        var mediapicker = (Microsoft.Maui.Media.IMediaPicker)AvaloniaDefaults.CreateAvaloniaMediaPicker(platformProvider);
        var hapticFeedback = new AvaloniaHapticFeedback();
        var preferences = new AvaloniaPreferences();
        var fileSystem = new AvaloniaFileSystem();
        var webAuthenticator = (Microsoft.Maui.Authentication.IWebAuthenticator)AvaloniaDefaults.CreateAvaloniaWebAuthenticator(platformProvider);
#if LINUX_OS
        if (OperatingSystem.IsLinux())
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
        return;
#elif MACOS
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

#elif WINDOWS_OS && !IOS && !ANDROID && !BROWSER
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
            null!,
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
#elif BROWSER
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
        
#endif

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


    /// <summary>
    /// Fallback <see cref="IVersionTracking"/> implementation used when the platform's default
    /// version tracking cannot be constructed (e.g. outside a fully initialized MAUI context).
    /// Every member throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class VersionTrackingDefault : IVersionTracking
    {
        /// <inheritdoc/>
        public bool IsFirstLaunchEver => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsFirstLaunchForCurrentVersion => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsFirstLaunchForCurrentBuild => throw new NotImplementedException();

        /// <inheritdoc/>
        public string CurrentVersion => throw new NotImplementedException();

        /// <inheritdoc/>
        public string CurrentBuild => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? PreviousVersion => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? PreviousBuild => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? FirstInstalledVersion => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? FirstInstalledBuild => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<string> VersionHistory => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<string> BuildHistory => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsFirstLaunchForBuild(string build)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsFirstLaunchForVersion(string version)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Track()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Opens a readable stream for the specified file, using the Avae-specific implementation
    /// when available, or falling back to the standard MAUI Essentials behavior.
    /// </summary>
    /// <param name="file">The file to open.</param>
    /// <param name="overridesMauiPlatform">
    /// Reserved for future use; does not currently affect behavior beyond selecting the Avae-specific path when available.
    /// </param>
    /// <returns>A task that resolves to a readable stream for the file's contents.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is <see langword="null"/>.</exception>
    public static Task<Stream> OpenReadAsync(this FileBase file, bool overridesMauiPlatform = true)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file is IAvaeFileResult avaeFileResult)
            return avaeFileResult.OpenFileStreamAsync();
        return file.OpenReadAsync();
    }

    /// <summary>
    /// Composes an email with the specified files attached, using the Avae-specific implementation
    /// when available, or converting the files to standard <see cref="EmailAttachment"/>s otherwise.
    /// </summary>
    /// <param name="email">The email service to compose with.</param>
    /// <param name="files">The files to attach to the email.</param>
    /// <param name="message">The email message to compose.</param>
    /// <returns>A task representing the asynchronous compose operation.</returns>
    public static Task ComposeAsync(this IEmail email, IEnumerable<FileBase> files, EmailMessage message)
    {
        if (email is IAvaeEmail avae)
        {
            return avae.ComposeAsync(files, message);
        }
        else
        {
            var attachments = new List<EmailAttachment>();
            foreach (var file in files ?? [])
            {
                attachments.Add(new EmailAttachment(file.FullPath));
            }
            message.Attachments = attachments;
            return email.ComposeAsync(message);
        }
    }

    /// <summary>
    /// Requests a native share of the specified files, using the Avae-specific implementation
    /// when available, or converting the files to a standard <see cref="ShareMultipleFilesRequest"/> otherwise.
    /// </summary>
    /// <param name="share">The share service to request with.</param>
    /// <param name="title">The title shown in the share dialog.</param>
    /// <param name="files">The files to share.</param>
    /// <returns>A task representing the asynchronous share operation.</returns>
    public static Task RequestAsync(this IShare share, string title, IEnumerable<FileBase> files)
    {
        if (share is IAvaeShare avae)
        {
            return avae.RequestAsync(title, files);
        }
        else
        {
            // Execute the native share request with the converted files
            return share.RequestAsync(new ShareMultipleFilesRequest()
            {
                Title = title,
                Files = [.. (files ?? []).Select(f => new ShareFile(f))]
            });
        }
    }
}