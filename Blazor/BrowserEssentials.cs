using Microsoft.JSInterop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Avae.Essentials;

/// <summary>
/// Entry point for the browser Essentials implementations. The JS interop module must be
/// imported once before any Essentials API is used — call <see cref="InitializeAsync"/>
/// (and await it) during app startup.
/// </summary>
public static class BrowserEssentials
{
    private static readonly TaskFactory _taskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func) =>
        _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

    public static void RunSync(Func<Task> func)
        => _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();



    const string Module = BrowserEssentials.ModuleName;

    static IJSObjectReference ModuleRef(IJSRuntime js)
    {
        BrowserEssentials.EnsureInitialized();
        return BrowserEssentials.ReferenceModule;
    }

    // ───────────────────────── Preferences (localStorage) ─────────────────────────
    internal static string? PrefGet(IJSRuntime js, string key)
    {
        return _isWasm ?
          ((IJSInProcessRuntime)js).Invoke<string?>("prefGet", key) :
          RunSync(async () => await ModuleRef(js).InvokeAsync<string?>("prefGet", key));
    }
    internal static void PrefSet(IJSRuntime js, string key, string value)
    {
        if (_isWasm)
            ((IJSInProcessRuntime)js).InvokeVoid("prefSet", key, value);
        else
            RunSync(async () => await ModuleRef(js).InvokeVoidAsync("prefSet", key, value));
    }
    internal static void PrefRemove(IJSRuntime js, string key)
    {
        if (_isWasm)
            ((IJSInProcessRuntime)js).InvokeVoid("prefRemove", key);
        else
            RunSync(async () => await ModuleRef(js).InvokeVoidAsync("prefRemove", key));
    }
    internal static string[] PrefKeys(IJSRuntime js, string prefix)
    {
        return _isWasm ?
          ((IJSInProcessRuntime)js).Invoke<string[]>("prefKeys", prefix) :
          RunSync(async () => await ModuleRef(js).InvokeAsync<string[]>("prefKeys", prefix));
    }
    // ───────────────────────── Secure storage ─────────────────────────
    internal static async Task SecureSetAsync(IJSRuntime js, string key, string value) =>
        await ModuleRef(js).InvokeVoidAsync("secureSet", key, value);

    internal static async Task<string?> SecureGetAsync(IJSRuntime js, string key) =>
        await ModuleRef(js).InvokeAsync<string?>("secureGet", key);

    // ───────────────────────── Clipboard ─────────────────────────
    internal static async Task ClipboardWriteTextAsync(IJSRuntime js, string text) =>
        await ModuleRef(js).InvokeVoidAsync("clipboardWriteText", text);

    internal static async Task<string> ClipboardReadTextAsync(IJSRuntime js) =>
        await ModuleRef(js).InvokeAsync<string>("clipboardReadText");

    // ───────────────────────── Connectivity ─────────────────────────
    internal static bool IsOnline(IJSRuntime js)
    {
        if (ReferenceModule is IJSInProcessObjectReference sync)
            return sync.Invoke<bool>("isOnline");
        return RunSync(async () => await (await Modules(js)).InvokeAsync<bool>("isOnline"));
    }

    internal static string GetConnectionType(IJSRuntime js)
    {
        if (ReferenceModule is IJSInProcessObjectReference sync)
            return sync.Invoke<string>("getConnectionType");
        return RunSync(async () => await (await Modules(js)).InvokeAsync<string>("getConnectionType"));
    }

    // Callbacks require a DotNetObjectReference — see the callbacks section below
    internal static async Task WatchConnectivityAsync(
        IJSRuntime js,
        DotNetObjectReference<ConnectivityCallback> callbackRef) =>
        await ModuleRef(js).InvokeVoidAsync("watchConnectivity", callbackRef);

    // ───────────────────────── Device info ─────────────────────────
    internal static string GetDeviceInfo(IJSRuntime js) =>
            _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<string>("getDeviceInfo") :
        RunSync(async () => await ModuleRef(js).InvokeAsync<string>("getDeviceInfo"));


    // ───────────────────────── Display ─────────────────────────
    internal static string GetDisplayInfo(IJSRuntime js) =>
                _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<string>("getDisplayInfo") :
        RunSync(async () => await ModuleRef(js).InvokeAsync<string>("getDisplayInfo"));

    internal static async Task WatchDisplayAsync(
        IJSRuntime js,
        DotNetObjectReference<DisplayCallback> callbackRef) =>
        await ModuleRef(js).InvokeVoidAsync("watchDisplay", callbackRef);

    internal static async Task<bool> SetWakeLockAsync(IJSRuntime js, bool enabled) =>
        await ModuleRef(js).InvokeAsync<bool>("setWakeLock", enabled);

    internal static bool GetWakeLock(IJSRuntime js) =>
        _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<bool>("getWakeLock") :
        RunSync(async () => await ModuleRef(js).InvokeAsync<bool>("getWakeLock"));
    
    // ───────────────────────── App info / theme ─────────────────────────
    internal static async Task<string> GetAppInfoAsync(IJSRuntime js) =>
        await ModuleRef(js).InvokeAsync<string>("getAppInfo");

    internal static async Task<bool> PrefersDarkAsync(IJSRuntime js) =>
        await ModuleRef(js).InvokeAsync<bool>("prefersDark");

    // ───────────────────────── Geolocation ─────────────────────────
    internal static async Task<string> GeoGetCurrentPositionAsync(
        IJSRuntime js, bool enableHighAccuracy, double timeoutMs) =>
        await ModuleRef(js).InvokeAsync<string>(
            "geoGetCurrentPosition", enableHighAccuracy, timeoutMs);

    internal static async Task<int> GeoWatchStartAsync(
        IJSRuntime js,
        bool enableHighAccuracy,
        DotNetObjectReference<GeoCallback> callbackRef) =>
        await ModuleRef(js).InvokeAsync<int>(
            "geoWatchStart", enableHighAccuracy, callbackRef);

    internal static async Task GeoWatchStopAsync(IJSRuntime js, int watchId) =>
        await ModuleRef(js).InvokeVoidAsync("geoWatchStop", watchId);

    // ───────────────────────── Battery ─────────────────────────
    internal static async Task<string?> BatteryStartAsync(
        IJSRuntime js,
        DotNetObjectReference<BatteryCallback> callbackRef) =>
        await ModuleRef(js).InvokeAsync<string?>("batteryStart", callbackRef);

    // ───────────────────────── Vibration ─────────────────────────
    internal static bool VibrationIsSupported(IJSRuntime js) =>
               _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<bool>("vibrationIsSupported") :
        RunSync(async () => await ModuleRef(js).InvokeAsync<bool>("vibrationIsSupported"));


    internal static async Task VibrateAsync(IJSRuntime js, double durationMs) =>
        await ModuleRef(js).InvokeVoidAsync("vibrate", durationMs);

    // ───────────────────────── Share ─────────────────────────
    internal static bool ShareIsSupported(IJSRuntime js) =>
                   _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<bool>("shareIsSupported") :
        RunSync(async () => await ModuleRef(js).InvokeAsync<bool>("shareIsSupported"));


    internal static async Task ShareAsync(
        IJSRuntime js, string? title, string? text, string? url) =>
        await ModuleRef(js).InvokeVoidAsync("share", title, text, url);

    internal static async Task ShareFilesAsync(
        IJSRuntime js,
        string? title,
        string namesJson,
        string typesJson,
        string base64Json) =>
        await ModuleRef(js).InvokeVoidAsync(
            "shareFiles", title, namesJson, typesJson, base64Json);

    // ───────────────────────── Launcher / browser ─────────────────────────
    internal static async Task<bool> OpenUrlAsync(IJSRuntime js, string url) =>
        await ModuleRef(js).InvokeAsync<bool>("openUrl", url);

    internal static async Task<bool> NavigateToAsync(IJSRuntime js, string url) =>
        await ModuleRef(js).InvokeAsync<bool>("navigateTo", url);

    internal static async Task<bool> OpenFileBlobAsync(
        IJSRuntime js, string base64, string? contentType, string name) =>
        await ModuleRef(js).InvokeAsync<bool>(
            "openFileBlob", base64, contentType, name);

    // ───────────────────────── File picker ─────────────────────────
    internal static async Task<string> PickFilesAsync(
        IJSRuntime js, string? accept, bool multiple) =>
        await ModuleRef(js).InvokeAsync<string>("pickFiles", accept, multiple);

    // ───────────────────────── Text to speech ─────────────────────────
    internal static async Task<string> SpeechGetVoicesAsync(IJSRuntime js) =>
        await ModuleRef(js).InvokeAsync<string>("speechGetVoices");

    internal static async Task SpeakAsync(
        IJSRuntime js,
        string text,
        string? lang,
        double pitch,
        double rate,
        double volume) =>
        await ModuleRef(js).InvokeVoidAsync(
            "speak", text, lang, pitch, rate, volume);

    internal static void SpeechCancel(IJSRuntime js)
    {
        if (_isWasm)
            ((IJSInProcessRuntime)js).InvokeVoid("speechCancel");
        else
            RunSync(async () => await ModuleRef(js).InvokeVoidAsync("speechCancel"));
    }
    // ───────────────────────── Sensors ─────────────────────────
    internal static bool SensorIsSupported(
        IJSRuntime js, string kind) =>
                  _isWasm ?
        ((IJSInProcessRuntime)js).Invoke<bool>("sensorIsSupported", kind) :
        RunSync(async () => await ModuleRef(js).InvokeAsync<bool>("sensorIsSupported", kind));


    internal static async Task<bool> SensorStartAsync(
        IJSRuntime js,
        string kind,
        DotNetObjectReference<SensorCallback> callbackRef) =>
        await ModuleRef(js).InvokeAsync<bool>("sensorStart", kind, callbackRef);

    internal static async Task SensorStopAsync(IJSRuntime js, string kind) =>
        await ModuleRef(js).InvokeVoidAsync("sensorStop", kind);

    // ───────────────────────── App package files ─────────────────────────
    internal static async Task<string?> FetchAppFileAsync(
        IJSRuntime js, string path) =>
        await ModuleRef(js).InvokeAsync<string?>("fetchAppFile", path);

    internal static async Task<bool> AppFileExistsAsync(
        IJSRuntime js, string path) =>
        await ModuleRef(js).InvokeAsync<bool>("appFileExists", path);

    // ───────────────────────── Screen reader ─────────────────────────
    internal static async Task AnnounceAsync(IJSRuntime js, string text) =>
        await ModuleRef(js).InvokeVoidAsync("announce", text);


    internal const string ModuleName = "BrowserEssentials";

    private static Task<IJSObjectReference>? initTask;
    private static bool _isWasm;
    private static readonly object _lock = new();

    /// <summary>
    /// Imports the interop JavaScript module. Idempotent; subsequent calls return the same task.
    /// </summary>
    /// <param name="js">The <see cref="IJSRuntime"/> instance (injected from DI).</param>
    /// <param name="moduleUrl">
    /// Optional URL of the BrowserEssentials.js module. When null (the default), the module
    /// embedded in this assembly is imported via a data: URL. Pass an explicit URL when the
    /// app's Content-Security-Policy disallows data: script sources — copy
    /// BrowserEssentials.js into the app's wwwroot folder and point this at it
    /// (e.g. "./_content/YourPackage/BrowserEssentials.js").
    /// </param>
    public static Task<IJSObjectReference> InitializeAsync(
        IJSRuntime js,
        bool isWasm,
        string? moduleUrl = null)
    {
        _isWasm = isWasm;
        // Fast path — already initialized
        if (initTask is { IsCompletedSuccessfully: true })
            return initTask;

        lock (_lock)
        {
            return initTask ??= InitializeCoreAsync(js, moduleUrl);
        }
    }

    private static readonly ConditionalWeakTable<IJSRuntime, Task<IJSObjectReference>> _modules = new();

    private static async Task<IJSObjectReference> Modules(IJSRuntime js)
    {
        try
        {
            return await CircuitServiceAccessor.Runtime!.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Avae.Essentials/BrowserEssentials.js");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Import failed: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public static void SetModules(IJSRuntime js, Task<IJSObjectReference> task)
        => _modules.AddOrUpdate(js, task);

    private static async Task<IJSObjectReference> InitializeCoreAsync(
        IJSRuntime js,
        string? moduleUrl)
    {
        if (moduleUrl is null)
        {
            using var stream = typeof(BrowserEssentials).Assembly
                .GetManifestResourceStream("BrowserEssentials.js")
                ?? throw new InvalidOperationException(
                    "Embedded resource 'BrowserEssentials.js' not found.");

            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory).ConfigureAwait(false);
            moduleUrl = "data:text/javascript;base64,"
                      + Convert.ToBase64String(memory.ToArray());
        }

        // Blazor's equivalent of JSHost.ImportAsync
        return await js.InvokeAsync<IJSObjectReference>("import", moduleUrl)
                       .ConfigureAwait(false);
    }

    /// <summary>True once <see cref="InitializeAsync"/> has completed.</summary>
    public static bool IsInitialized =>
        initTask is { IsCompletedSuccessfully: true };

    /// <summary>The imported module reference (throws if not initialized).</summary>
    internal static IJSObjectReference ReferenceModule
    {
        get
        {
            EnsureInitialized();
            return initTask!.Result;
        }
    }

    /// <summary>
    /// Throws when the module has not been imported yet. Used by synchronous API surfaces
    /// that cannot await initialization themselves.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException(
                "Browser Essentials is not initialized. Call " +
                "'await BrowserEssentials.InitializeAsync(js)' during app startup " +
                "before using Essentials APIs.");
    }

    ///// <summary>
    ///// Awaited by asynchronous API surfaces — starts initialization on demand if the app
    ///// did not call <see cref="InitializeAsync"/> explicitly.
    ///// </summary>
    //internal static Task<IJSObjectReference> WhenInitializedAsync(IJSRuntime js)
    //    => InitializeAsync(js);
}