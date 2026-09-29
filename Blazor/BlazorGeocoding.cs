using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

/// <summary>Geocoding requires a service backend; none is available in the browser.</summary>
public class BlazorGeocoding : IGeocoding
{
    public Task<IEnumerable<Placemark>> GetPlacemarksAsync(double latitude, double longitude) =>
        throw new FeatureNotSupportedException("Geocoding is not supported in the browser.");

    public Task<IEnumerable<Location>> GetLocationsAsync(string address) =>
        throw new FeatureNotSupportedException("Geocoding is not supported in the browser.");
}
