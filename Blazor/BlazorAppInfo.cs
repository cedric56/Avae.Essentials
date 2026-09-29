using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using System.Reflection;

namespace Avae.Essentials;

/// <summary>
/// App info sourced from the hosting document (title, origin, theme) and the entry assembly
/// (version). JS-backed values are fetched once via <see cref="InitializeAsync"/> and kept
/// current by browser theme-change events, so the synchronous <see cref="IAppInfo"/> surface
/// never touches JS interop directly.
/// </summary>
public sealed class BlazorAppInfo(IJSRuntime js) : IAppInfo, IAsyncDisposable
{
    private static readonly Version AssemblyVersion =
        Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(1, 0);

    private sealed record AppInfoSnapshot(string Title, string Hostname, bool Rtl, bool PrefersDark);

    private IJSObjectReference? _module;
    private DotNetObjectReference<BlazorAppInfo>? _ref;
    private Task? _init;

    private string _packageName = string.Empty;
    private string _name = Assembly.GetEntryAssembly()?.GetName().Name ?? "App";
    private LayoutDirection _layoutDirection = LayoutDirection.LeftToRight;
    private AppTheme _requestedTheme = AppTheme.Light;

    /// <summary>
    /// Fetches the JS-backed values once and subscribes to theme changes.
    /// Idempotent: safe to call more than once. Must complete before any property is read.
    /// </summary>
    public Task InitializeAsync(string moduleUrl) => _init ??= InitCoreAsync(moduleUrl);

    private async Task InitCoreAsync(string moduleUrl)
    {
        _module = await js.InvokeAsync<IJSObjectReference>("import", moduleUrl);

        var snapshot = await _module.InvokeAsync<AppInfoSnapshot>("appInfoGet");
        Apply(snapshot);

        _ref = DotNetObjectReference.Create(this);
        await _module.InvokeVoidAsync("appInfoSubscribeTheme", _ref);
    }

    [JSInvokable]
    public void OnThemeChanged(bool prefersDark)
        => _requestedTheme = prefersDark ? AppTheme.Dark : AppTheme.Light;

    private void Apply(AppInfoSnapshot s)
    {
        _packageName = s.Hostname;
        _name = string.IsNullOrEmpty(s.Title)
            ? Assembly.GetEntryAssembly()?.GetName().Name ?? "App"
            : s.Title;
        _layoutDirection = s.Rtl ? LayoutDirection.RightToLeft : LayoutDirection.LeftToRight;
        _requestedTheme = s.PrefersDark ? AppTheme.Dark : AppTheme.Light;
    }

    // ───────────────────────── IAppInfo (sync, cached) ─────────────────────────

    public string PackageName => _packageName;
    public string Name => _name;
    public string VersionString => AssemblyVersion.ToString();
    public Version Version => AssemblyVersion;
    public string BuildString =>
        AssemblyVersion.Revision >= 0 ? AssemblyVersion.Revision.ToString() : "0";
    public AppTheme RequestedTheme => _requestedTheme;
    public AppPackagingModel PackagingModel => AppPackagingModel.Unpackaged;
    public LayoutDirection RequestedLayoutDirection => _layoutDirection;

    public void ShowSettingsUI() =>
        throw new FeatureNotSupportedException("Browsers do not expose an app settings UI.");

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
        if (_module is null) return;
        try
        {
            await _module.InvokeVoidAsync("appInfoUnsubscribeTheme");
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException) { /* circuit already gone */ }
    }
}