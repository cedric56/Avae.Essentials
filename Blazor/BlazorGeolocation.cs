using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

/// <summary>Geolocation backed by navigator.geolocation (permission-prompted by the browser).</summary>
public sealed class BlazorGeolocation : IGeolocation
{
    public sealed record Payload(double Latitude, double Longitude, double? Accuracy,
        double? Altitude, double? AltitudeAccuracy, double? Heading, double? Speed, double Timestamp);

    private DotNetObjectReference<BlazorGeolocation>? _ref;
    private Location? _lastKnownLocation;
    private int _watchId = -1;

    public bool IsListeningForeground => _watchId >= 0;

    // Browsers only reveal permission state at request time; assume available.
    public bool IsEnabled => true;

    public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
    public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;

    public Task<Location?> GetLastKnownLocationAsync() => Task.FromResult(_lastKnownLocation);

    public async Task<Location?> GetLocationAsync(GeolocationRequest request, CancellationToken cancelToken = default)
    {
        cancelToken.ThrowIfCancellationRequested();
        try
        {
            var payload = await BlazorEssentials.Module.InvokeAsync<Payload>("geoGetCurrentPosition", cancelToken, UseHighAccuracy(request), request.Timeout.TotalMilliseconds);
            if (payload != null)
                return _lastKnownLocation = ToLocation(payload);
        }
        catch (JSException ex) when (ex.Message.Contains("permission", StringComparison.OrdinalIgnoreCase))
        {
            throw new PermissionException("Geolocation permission was denied by the user or browser policy.");
        }
        catch (JSException ex) when (ex.Message.Contains("unsupported", StringComparison.OrdinalIgnoreCase))
        {
            throw new FeatureNotSupportedException("Geolocation is not supported by this browser.");
        }
        catch (JSException ex) when (ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            return null; // matches MAUI's behavior of returning null on timeout
        }
        return null;
    }

    public async Task<bool> StartListeningForegroundAsync(GeolocationListeningRequest request)
    {
        if (IsListeningForeground)
            throw new InvalidOperationException("Already listening for location changes.");

        _ref = DotNetObjectReference.Create(this);
        var highAccuracy = request.DesiredAccuracy is GeolocationAccuracy.Best or GeolocationAccuracy.High;

        try
        {
            _watchId = await BlazorEssentials.Module.InvokeAsync<int>("geoWatchStart", _ref, highAccuracy);
        }
        catch (JSDisconnectedException)
        {
            _watchId = -1;
        }

        return _watchId >= 0;
    }

    public async void StopListeningForeground()
    {
        if (!IsListeningForeground)
            return;

        var id = _watchId;
        _watchId = -1;

        await BlazorEssentials.Module.InvokeVoidAsync("geoWatchStop", id);
    }

    [JSInvokable]
    public void OnLocationChanged(Payload payload)
    {
        var location = ToLocation(payload);
        _lastKnownLocation = location;
        LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(location));
    }

    [JSInvokable]
    public void OnListeningFailed(string errorCode)
    {
        var error = errorCode switch
        {
            "permission" => GeolocationError.Unauthorized,
            "timeout" => GeolocationError.PositionUnavailable,
            _ => GeolocationError.PositionUnavailable,
        };
        ListeningFailed?.Invoke(this, new GeolocationListeningFailedEventArgs(error));
    }

    private static bool UseHighAccuracy(GeolocationRequest request) =>
        request.DesiredAccuracy is GeolocationAccuracy.Best or GeolocationAccuracy.High;

    private static Location ToLocation(Payload p) => new(p.Latitude, p.Longitude)
    {
        Accuracy = p.Accuracy,
        Altitude = p.Altitude,
        VerticalAccuracy = p.AltitudeAccuracy,
        Course = p.Heading,
        Speed = p.Speed,
        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)p.Timestamp),
    };
}