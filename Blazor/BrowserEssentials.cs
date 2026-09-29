using Microsoft.JSInterop;

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



    static IJSObjectReference ModuleRef(IJSRuntime js)
    {
        BrowserEssentials.EnsureInitialized();
        return BrowserEssentials.ReferenceModule;
    }
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
}