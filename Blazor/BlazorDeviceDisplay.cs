using Microsoft.JSInterop;
using Microsoft.Maui.Devices;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Avae.Essentials;

/// <summary>
/// Display info from window.innerWidth/Height and devicePixelRatio. KeepScreenOn uses the
/// Screen Wake Lock API where available (secure contexts, most modern browsers), re-acquired
/// automatically when the page becomes visible again, since browsers release wake locks
/// whenever the tab is hidden.
/// </summary>
public sealed class BlazorDeviceDisplay : IDeviceDisplay, IAsyncDisposable
{
    public sealed record Snapshot(double Width, double Height, double PixelRatio, string OrientationType, bool WakeLockActive);
    public sealed record ChangeSnapshot(double Width, double Height, double PixelRatio, string OrientationType);

    private DotNetObjectReference<BlazorDeviceDisplay>? _ref;
    private bool _watching;

    private DisplayInfo _display = new(0, 0, 1, DisplayOrientation.Unknown, DisplayRotation.Unknown);
    private bool _wakeLockActive;

    private event EventHandler<DisplayInfoChangedEventArgs>? _mainDisplayInfoChanged;

    public async Task InitializeAsync()
    {
        var snapshot = await BlazorEssentials.Module.InvokeAsync<Snapshot?>("ddGetSnapshot");
        if (snapshot is not null)
        {
            Apply(snapshot.Width, snapshot.Height, snapshot.PixelRatio, snapshot.OrientationType);
            _wakeLockActive = snapshot.WakeLockActive;
        }
    }

    // ───────────────────────── IDeviceDisplay ─────────────────────────

    public bool KeepScreenOn
    {
        get => _wakeLockActive;
        set
        {
            EnsureWatching();
            _ = SetWakeLockAsync(value);
        }
    }

    public DisplayInfo MainDisplayInfo => _display;

    public event EventHandler<DisplayInfoChangedEventArgs>? MainDisplayInfoChanged
    {
        add { EnsureWatching(); _mainDisplayInfoChanged += value; }
        remove => _mainDisplayInfoChanged -= value;
    }

    // ───────────────────────── internals ─────────────────────────
    private Task? _ensureWatchingTask;

    private void EnsureWatching()
    {
        _ensureWatchingTask ??= EnsureWatchingAsync();
    }

    private async Task EnsureWatchingAsync()
    {
        try
        {
            if (_watching) return;
            _watching = true;
            _ref?.Dispose();
            _ref = DotNetObjectReference.Create(this);
            await BlazorEssentials.Module.InvokeVoidAsync("ddSubscribe", _ref);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _ensureWatchingTask = null;
        }
    }

    private async Task SetWakeLockAsync(bool on)
    {
        EnsureWatching();
        await BlazorEssentials.Module.InvokeVoidAsync("ddSetWakeLock", _ref, on);
    }

    [JSInvokable]
    public void OnWakeLockChanged(bool active) => _wakeLockActive = active;

    [JSInvokable]
    public void OnDisplayChanged(ChangeSnapshot s)
    {
        Apply(s.Width, s.Height, s.PixelRatio, s.OrientationType);
        _mainDisplayInfoChanged?.Invoke(this, new DisplayInfoChangedEventArgs(_display));
    }

    private void Apply(double width, double height, double pixelRatio, string orientationType)
    {
        var orientation = orientationType.StartsWith("portrait", StringComparison.Ordinal)
            ? DisplayOrientation.Portrait
            : DisplayOrientation.Landscape;

        var rotation = orientationType switch
        {
            "portrait-primary" or "landscape-primary" => DisplayRotation.Rotation0,
            "landscape-secondary" or "portrait-secondary" => DisplayRotation.Rotation180,
            _ => DisplayRotation.Unknown,
        };

        _display = new DisplayInfo(width, height, pixelRatio, orientation, rotation);
    }

    public async ValueTask DisposeAsync()
    {
        await BlazorEssentials.Module.InvokeVoidAsync("ddUnsubscribe");

        _ref?.Dispose();
        _ref = null;
    }
}