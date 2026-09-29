using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public static class BlazorEssentials
{
    private static string? _moduleUrl = string.Empty;
    private static Task<IJSObjectReference>? initTask;
    private static readonly object _lock = new();

    /// <summary>True once <see cref="InitializeAsync"/> has completed.</summary>
    public static bool IsInitialized =>
        initTask is { IsCompletedSuccessfully: true };

    /// <summary>
    /// Throws when the module has not been imported yet. Used by synchronous API surfaces
    /// that cannot await initialization themselves.
    /// </summary>
    internal static void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException(
                "Blazor Essentials is not initialized. Call " +
                "'await BlazorEssentials.InitializeAsync(js, moduleurl)' during app startup " +
                "before using Essentials APIs.");
    }

    internal static Task<IJSObjectReference> InitializeAsync(IJSRuntime? js)
    {
        // Fast path — already initialized
        if (initTask is { IsCompletedSuccessfully: true })
            return initTask;

        lock (_lock)
        {
            return initTask ??= InvokeCoreAsync(js);
        }
    }

    public static bool IsWasm { get; private set; }

    public static async Task InitializeAsync(
       IServiceProvider provider,        
       string moduleUrl,
       bool isWasm = true)
    {
        IsWasm = isWasm;

        _moduleUrl = moduleUrl;

        var js = provider.GetRequiredService<IJSRuntime>();
        CircuitServiceAccessor.Provider = provider;
        CircuitServiceAccessor.Runtime = js;

        await InitializeAsync(js);

        var connectivity = (BlazorConnectivity)provider.GetRequiredService<IConnectivity>();
        await connectivity.InitializeAsync(moduleUrl);

        var appInfo = (BlazorAppInfo)provider.GetRequiredService<IAppInfo>();
        await appInfo.InitializeAsync(moduleUrl);

        var feedback = (BlazorHapticFeedback)provider.GetRequiredService<IHapticFeedback>();
        await feedback.InitializeAsync();

        var preferences = (BlazorPreferences)provider.GetRequiredService<IPreferences>();
        await preferences.InitializeAsync();

        var storage = (BlazorSecureStorage)provider.GetRequiredService<ISecureStorage>();
        await storage.InitializeAsync();
    }

    internal static async Task<IJSObjectReference> InvokeCoreAsync(IJSRuntime? js)
    {
        if (js == null)
            throw new InvalidOperationException();

        return await js.InvokeAsync<IJSObjectReference>("import", _moduleUrl)
                       .ConfigureAwait(false);
    }
}
