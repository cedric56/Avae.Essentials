using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Avae.Essentials;

/// <summary>
/// App info sourced from the hosting document (title, origin) and the entry assembly (version).
/// The JS-backed values are fetched once via <see cref="InitializeAsync"/> and cached so the
/// synchronous <see cref="IAppInfo"/> surface can return them.
/// </summary>
public class BrowserAppInfo : IAppInfo
{
    static readonly Version AssemblyVersion =
        Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(1, 0);

    // Cached values populated by InitializeAsync
    private string _packageName = string.Empty;
    private string _name = "App";
    private LayoutDirection _layoutDirection = LayoutDirection.LeftToRight;
    private AppTheme _requestedTheme = AppTheme.Light;

    /// <summary>
    /// Fetches the JS-backed values once. Must be awaited during app startup, before any
    /// property on this class is read.
    /// </summary>
    public async Task InitializeAsync(IJSRuntime js)
    {
        //var module = BrowserEssentials.ReferenceModule;

        //// App info (title, hostname, rtl)
        //var infoJson = await module.InvokeAsync<string>("getAppInfo").ConfigureAwait(false);
        //using (var doc = JsonDocument.Parse(infoJson))
        //{
        //    _packageName = doc.RootElement.TryGetProperty("hostname", out var h)
        //        ? h.GetString() ?? string.Empty
        //        : string.Empty;

        //    var title = doc.RootElement.TryGetProperty("title", out var t)
        //        ? t.GetString()
        //        : null;

        //    _name = string.IsNullOrEmpty(title)
        //        ? Assembly.GetEntryAssembly()?.GetName().Name ?? "App"
        //        : title!;

        //    _layoutDirection = doc.RootElement.TryGetProperty("rtl", out var rtl)
        //                        && rtl.GetBoolean()
        //        ? LayoutDirection.RightToLeft
        //        : LayoutDirection.LeftToRight;
        //}

        //// Theme
        //var prefersDark = await module
        //    .InvokeAsync<bool>("prefersDark")
        //    .ConfigureAwait(false);
        //_requestedTheme = prefersDark ? AppTheme.Dark : AppTheme.Light;
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
        throw new FeatureNotSupportedException(
            "Browsers do not expose an app settings UI.");
}