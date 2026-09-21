using Microsoft.Maui.Devices.Sensors;
using System.Runtime.Versioning;
using Windows.Services.Maps;

namespace Avae.Essentials;

[SupportedOSPlatform("windows10.0.10240")]
static partial class LocationExtensions
{
    internal static IEnumerable<Placemark> ToPlacemarks(this IEnumerable<MapLocation> mapLocations)
    {
        return mapLocations.Select(address => new Placemark
        {
            Location = address.ToLocation(),
            FeatureName = address.DisplayName,
            PostalCode = address.Address.PostCode,
            CountryCode = address.Address.CountryCode,
            CountryName = address.Address.Country,
            Thoroughfare = address.Address.Street,
            SubThoroughfare = address.Address.StreetNumber,
            Locality = address.Address.Town,
            AdminArea = address.Address.Region,
            SubAdminArea = address.Address.District,
            SubLocality = address.Address.Neighborhood
        });
    }

    internal static IEnumerable<Placemark> ToPlacemarks(this MapLocationFinderResult result) =>
        result?.ToPlacemarks() ?? [];
}
