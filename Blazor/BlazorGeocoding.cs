using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices.Sensors;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avae.Essentials
{
    [JsonSerializable(typeof(PlacemarkResponseInterop))]
    [JsonSerializable(typeof(IEnumerable<NominatimResponse>))]
    internal partial class BlazorGeocodingSerializerContext : JsonSerializerContext
    {

    }

    internal class BlazorGeocoding(CircuitServiceAccessor circuitServiceAccessor) : IGeocoding
    {
        public async Task<IEnumerable<Location>> GetLocationsAsync(string address)
        {
            var httpClient = circuitServiceAccessor.GetRequiredService<HttpClient>();
            using var client = httpClient ?? new HttpClient();
            string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";
            var response = await client.GetStringAsync(url);
            var data = JsonSerializer.Deserialize(response, BlazorGeocodingSerializerContext.Default.IEnumerableNominatimResponse);
            if (data is null)
            {
                return [];
            }
            return [.. data.Select(d =>
            {
                return new Location()
                {
                    Latitude =  d.LatValue,
                    Longitude = d.LonValue
                };

            })];
        }

        public async Task<IEnumerable<Placemark>> GetPlacemarksAsync(double latitude, double longitude)
        {
            var httpClient = circuitServiceAccessor.GetRequiredService<HttpClient>();
            using var client = httpClient ?? new HttpClient();
            // Nominatim reverse geocoding URL
            string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}&addressdetails=1";

            // Make the request to the Nominatim API
            var response = await client.GetStringAsync(url);

            // Parse the JSON response
            var data = JsonSerializer.Deserialize(response, BlazorGeocodingSerializerContext.Default.PlacemarkResponseInterop);

            if (data is null)
            {
                return [new()];
            }

            // Return the full address (can also return specific components)
            return
            [
                new()
                {
                    CountryName = data.Address?.Country ?? string.Empty,
                    CountryCode = data.Address?.CcountryCode ?? string.Empty,
                    Location = new Location() { Latitude = data.LatValue, Longitude = data.LonValue },
                    FeatureName = data.Address?.Road ?? string.Empty,
                    Locality = data.Address?.Village ?? string.Empty,
                    PostalCode = data.Address?.Postcode ?? string.Empty
                }
            ];
        }
    }

    internal class NominatimResponse
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = null!;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = null!;


        public double LatValue => double.Parse(Lat, CultureInfo.InvariantCulture);
        public double LonValue => double.Parse(Lon, CultureInfo.InvariantCulture);
    }

    internal class Address
    {
        [JsonPropertyName("road")]
        public string? Road { get; set; }

        [JsonPropertyName("hamlet")]
        public string? Hamlet { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        [JsonPropertyName("municipality")]
        public string? Municipality { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("ISO3166-2-lvl6")]
        public string? ISO31662lvl6 { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("ISO3166-2-lvl4")]
        public string? ISO31662lvl4 { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("country_code")]
        public string? CcountryCode { get; set; }
    }

    internal class PlacemarkResponseInterop
    {
        [JsonPropertyName("place_id")]
        public int PlaceId { get; set; }

        [JsonPropertyName("licence")]
        public string? License { get; set; }

        [JsonPropertyName("osm_type")]
        public string? OsmType { get; set; }

        [JsonPropertyName("osm_id")]
        public int OsmId { get; set; }

        [JsonPropertyName("lat")]
        public string? Lat { get; set; }

        [JsonPropertyName("lon")]
        public string? Lon { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("place_rank")]
        public int PlaceRank { get; set; }

        [JsonPropertyName("importance")]
        public string? ImportanceStr { get; set; }

        [JsonPropertyName("addresstype")]
        public string? AddressType { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("address")]
        public Address? Address { get; set; }

        [JsonPropertyName("boundingbox")]
        public List<string>? boundingbox { get; set; }

        public double LatValue => double.Parse(Lat!, CultureInfo.InvariantCulture);
        public double LonValue => double.Parse(Lon!, CultureInfo.InvariantCulture);
        public double Importance => double.Parse(ImportanceStr!, CultureInfo.InvariantCulture);

    }
}
